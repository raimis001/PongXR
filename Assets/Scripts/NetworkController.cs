using Unity.Entities;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.NetCode;
using UnityEngine.InputSystem.XR;

public class NetworkController : MonoBehaviour
{
    public string Address = "127.0.0.1";
    public ushort NetworkPort = 7979;

    internal static string OldFrontendWorldName = string.Empty;
    XRHand connectionhand;
    public void OnHandServer(XRHand hand)
    {
        if (hand.triggerDown)
        {
            connectionhand = hand;
            StartServer();
        }
    }
    public void OnHandClient(XRHand hand)
    {
        if (!hand.triggerDown)
        {
            connectionhand = hand;
            StartClient();
        }
    }

    private void Start()
    {
        Application.runInBackground = true;
    }

    void OnConnected()
    {
        connectionhand.GetComponent<HandControll>().ShowText("Conected");
        DestroyLocalSimulationWorld();
    }


    void StartServer()
    {
        if (ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.ClientAndServer)
        {
            Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
            return;
        }

        var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        if (World.DefaultGameObjectInjectionWorld == null)
            World.DefaultGameObjectInjectionWorld = server;

        OnConnected();

        NetworkEndpoint ep = NetworkEndpoint.AnyIpv4.WithPort(NetworkPort);
        {
            using var drvQuery = server.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ep);
        }

        ep = NetworkEndpoint.LoopbackIpv4.WithPort(NetworkPort);
        {
            using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
        }
       // AddConnectionUISystemToUpdateList();
    }

    void StartClient()
    {
        var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        if (World.DefaultGameObjectInjectionWorld == null)
            World.DefaultGameObjectInjectionWorld = client;

        OnConnected();

        var ep = NetworkEndpoint.Parse(Address, NetworkPort);
        {
            using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
        }
        //AddConnectionUISystemToUpdateList();
    }

    void AddConnectionUISystemToUpdateList()
    {
        //foreach (var world in World.All)
        //{
           
        //}
    }

    static void DestroyLocalSimulationWorld()
    {
        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                OldFrontendWorldName = world.Name;
                world.Dispose();
                break;
            }
        }
    }
}
