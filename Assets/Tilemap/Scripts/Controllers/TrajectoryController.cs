using UnityEngine;
using System.Collections;
using System;

public delegate void TrajectoryChangedHandler(VehicleStatus status);

public enum VehicleStatus
{
    OpenDoors,
    CloseDoors,
    WillDepart,
    IsArriving,
    Gone,
    WillArrive
}

public class TrajectoryController : MonoBehaviour
{
    [SerializeField] public VisualPassenger visualPassenger;
    [SerializeField] public VisualStation visualStation;

    private GameObject startingCheckpoint;
    private GameObject dissapearCheckpoint;
    private GameObject stopCheckpoint;
    private GameObject decelerateCheckpoint;

    private GameObject movingVehicle;

    private float waitingTime;
    private float elapsedTime = 0;

    private float time = 300;
    private float speed = 0;

    private bool shouldDepart = false;
    private bool arrivingUniqueNotification = false;
    private bool departingUniqueNotification = false;
    private bool closingDoorsUniqueNotification = false;

    public event TrajectoryChangedHandler TrajectoryStatusChanged;

    bool ShouldDecelerate
    {
        get
        {
            return movingVehicle.GetComponent<SpriteRenderer>().bounds.Intersects(decelerateCheckpoint.GetComponent<BoxCollider>().bounds);
        }
    }

    bool ShouldStop
    {
        get
        {
            return movingVehicle.GetComponent<SpriteRenderer>().bounds.Intersects(stopCheckpoint.GetComponent<BoxCollider>().bounds);
        }
    }

    bool ShouldDissapear
    {
        get
        {
            return movingVehicle.GetComponent<SpriteRenderer>().bounds.Intersects(dissapearCheckpoint.GetComponent<BoxCollider>().bounds);
        }
    }

    private Vector2 defaultOrigin
    {
        get { return ViewPort.defaultOrigin; }
    }

    private Func<Grid<PassengerSeat>, int, int, PassengerSeat> gridDelegate
    {
        get { return (Grid<PassengerSeat> g, int x, int y) => new PassengerSeat(x, y, g); }
    }

    private FloatingTilemapVisual BuildRandomPassengersLayout()
    {
        Grid<PassengerSeat> grid = new Grid<PassengerSeat>(3, UnityEngine.Random.Range(5, 8), 9, defaultOrigin, gridDelegate);
        return new FloatingTilemapVisual(grid, visualPassenger, visualStation);
    }

    void GenerateVehicle()
    {
        speed = 0;
        elapsedTime = 0;
        waitingTime = UnityEngine.Random.Range(15, 25);
        arrivingUniqueNotification = false;
        closingDoorsUniqueNotification = false;
        departingUniqueNotification = false;
        shouldDepart = false;

        var floatingTilemap = BuildRandomPassengersLayout();

        movingVehicle = floatingTilemap.GeneratePassengerGrid();
        movingVehicle.transform.position = startingCheckpoint.transform.position;

        Physics.IgnoreCollision(movingVehicle.GetComponent<BoxCollider>(), stopCheckpoint.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(movingVehicle.GetComponent<BoxCollider>(), decelerateCheckpoint.GetComponent<BoxCollider>());
        Physics.IgnoreCollision(movingVehicle.GetComponent<BoxCollider>(), dissapearCheckpoint.GetComponent<BoxCollider>());

        TrajectoryStatusChanged?.Invoke(VehicleStatus.WillArrive);
    }

    // Use this for initialization
    void Awake()
    {
        startingCheckpoint = GameObject.FindGameObjectWithTag("trajectory-appear");
        startingCheckpoint.transform.position = new Vector3(startingCheckpoint.transform.position.x, startingCheckpoint.transform.position.y, 0);
        stopCheckpoint = GameObject.FindGameObjectWithTag("trajectory-stop");
        stopCheckpoint.transform.position = new Vector3(stopCheckpoint.transform.position.x, stopCheckpoint.transform.position.y, 0);
        decelerateCheckpoint = GameObject.FindGameObjectWithTag("trajectory-approaches");
        decelerateCheckpoint.transform.position = new Vector3(decelerateCheckpoint.transform.position.x, decelerateCheckpoint.transform.position.y, 0);
        dissapearCheckpoint = GameObject.FindGameObjectWithTag("trajectory-disappear");
        dissapearCheckpoint.transform.position = new Vector3(dissapearCheckpoint.transform.position.x, dissapearCheckpoint.transform.position.y, 0);

        GenerateVehicle();
    }

    // Update is called once per frame
    void Update()
    {
        if(movingVehicle == null) { return; }

        var distance = stopCheckpoint.GetComponent<BoxCollider>().bounds.center.y - movingVehicle.GetComponent<SpriteRenderer>().bounds.max.y;

        if (ShouldDissapear)
        {
            GameObject.DestroyImmediate(movingVehicle);
            TrajectoryStatusChanged?.Invoke(VehicleStatus.Gone);
            StartCoroutine(ReGenerateVehicle());
        }
        else if (shouldDepart)
        {
            movingVehicle.GetComponent<Rigidbody>().AddForce(new Vector2(0, 20 * Time.deltaTime), ForceMode.Impulse);

            if (!departingUniqueNotification)
            {
                TrajectoryStatusChanged?.Invoke(VehicleStatus.WillDepart);
                departingUniqueNotification = true;
            }
        }
        else if (ShouldStop)
        {
            movingVehicle.GetComponent<Rigidbody>().velocity = Vector3.zero;

            if(elapsedTime == 0) { TrajectoryStatusChanged?.Invoke(VehicleStatus.OpenDoors); }
            
            if (Math.Abs(waitingTime-elapsedTime) <= 5) {
                if(!closingDoorsUniqueNotification)
                {
                    TrajectoryStatusChanged?.Invoke(VehicleStatus.CloseDoors);
                    closingDoorsUniqueNotification = true;
                }
            }

            elapsedTime += Time.deltaTime;
            if (elapsedTime > waitingTime)
            {
                shouldDepart = true;
                elapsedTime = 0;
            }
        }
        else if (ShouldDecelerate)
        {
            speed = distance / time;
            movingVehicle.GetComponent<Rigidbody>().AddForce(new Vector2(0, -speed), ForceMode.VelocityChange);
        } else {
            speed = distance / time;
            movingVehicle.GetComponent<Rigidbody>().AddForce(new Vector2(0, speed), ForceMode.VelocityChange);

            if (!arrivingUniqueNotification)
            {
                TrajectoryStatusChanged?.Invoke(VehicleStatus.IsArriving);
                arrivingUniqueNotification = true;
            }
        }
    }

    IEnumerator ReGenerateVehicle()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(7, 15));
        GenerateVehicle();
    }
}
