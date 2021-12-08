using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class ProjBehaviorTree : MonoBehaviour
{

    public Light lamp;

    public GameObject participant1;
    public GameObject participant2;
    public GameObject participant3;
    public GameObject participant4;

    public Transform lightSwitchStandPoint;
    public GameObject lightSwitch;
    public InteractionObject lightSwitchIK;
    public Transform TVTurnStandPoint;
    public GameObject TVSwitch;
    public InteractionObject TVSwitchIK;

    public FullBodyBipedEffector hand;

    public Transform TVStandPoint;
    public Transform TVLookAtPoint;

    public Transform TVp1;
    public Transform TVp2;
    public Transform TVp3;

    public InteractionObject SofaIK1;
    public InteractionObject SofaIK2;
    public InteractionObject SofaIK3;

    public FullBodyBipedEffector butt;

    public GameObject canvasLight;
    public GameObject canvasTV;
    public Text bubbleTextL;
    public Text bubbleTextT;

    public Transform p1;
    public Transform p2;
    public Transform p3;

    public GameObject ball;


    BehaviorMecanim part1;
    BehaviorMecanim part2;
    BehaviorMecanim part3;
    public GameObject Player;

    private int angryCount = 0;



    private BehaviorAgent behaviorAgent;
    // Use this for initialization
    void Start ()
    {

        part1 = participant1.GetComponent<BehaviorMecanim> ();
        part2 = participant2.GetComponent<BehaviorMecanim> ();
        part3 = participant3.GetComponent<BehaviorMecanim> ();

        canvasLight.GetComponent<CanvasGroup>().alpha = 0;
        canvasTV.GetComponent<CanvasGroup>().alpha = 0;
        behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
        BehaviorManager.Instance.Register (behaviorAgent);
        behaviorAgent.StartBehavior ();

        Player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update ()
    {

    }
    /*
       protected Node ST_ApproachAndWait(Transform target)
       {
       Val<Vector3> position = Val.V (() => target.position);
       return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
       }
       */

    protected Node faceAndPoint(BehaviorMecanim part, GameObject facing, int time)
    {
        Val<Vector3> face = Val.V (() => facing.transform.position);
        return new Sequence
            (
             part.Node_OrientTowards(face),
             part.Node_HandAnimation("pointing", true),
             new LeafWait(time),
             part.Node_HandAnimation("pointing", false)
            );
    }


    protected Node LightOff(BehaviorMecanim part)
    {
        Val<Vector3> position = Val.V (() => lightSwitchStandPoint.position);
        Val<Vector3> face = Val.V (() => lightSwitch.transform.position);
        return new Sequence
            (
             part.Node_StopInteraction(butt),
             part.Node_GoTo(position),
             part.Node_OrientTowards(face),
             // part.Node_HandAnimation("pointing", true),
             part.Node_StartInteraction(hand, lightSwitchIK),
             new LeafWait(500),
             new LeafInvoke(() => lamp.enabled = !lamp.enabled),
             // part.Node_HandAnimation("pointing", false)
             part.Node_StopInteraction(hand)
            );
    }


    protected Node TVOnOff(BehaviorMecanim part)
    {
        Val<Vector3> position = Val.V (() => TVTurnStandPoint.position);
        Val<Vector3> face = Val.V (() => TVSwitch.transform.position);
        return new Sequence
            (
             part.Node_GoTo(position),
             part.Node_OrientTowards(face),
             // part.Node_HandAnimation("pointing", true),
             part.Node_StartInteraction(hand, TVSwitchIK),
             new LeafWait(500),
             // part.Node_HandAnimation("pointing", false)
             part.Node_StopInteraction(hand)
            );
    }

    protected Node TextOn(String speech, GameObject canvas, Text t)
    {
        //t.text = speech;
        return new Sequence
            (
             new LeafInvoke(() => t.text = speech),
             new LeafInvoke(() => canvas.GetComponent<CanvasGroup>().alpha = 1),
             new LeafWait(2000),
             new LeafInvoke(() => canvas.GetComponent<CanvasGroup>().alpha = 0)
            );

    }

    protected Node WatchTV(BehaviorMecanim part, Transform p, InteractionObject s)
    {
        Val<Vector3> tvpos = Val.V (() => TVLookAtPoint.position);
        Val<Vector3> standpos = Val.V (() => p.position);
        // Val<InteractionObject> sofapos = Val.V (() => s);
        //Val<float> dist = Val.V (() => 2.0f);
        return new Sequence(
                part.Node_GoTo (standpos),
                part.Node_OrientTowards (tvpos),
                part.Node_StartInteraction(butt, s)
                );
    }

    private void updatePos(Val<Vector3> v, GameObject part)
    {
        part.GetComponent<SteeringController> ().Target = v.Value;
    }

    protected Node StoryPause()
    {
        // A, B and C are now sitting on the sofa
        return new Sequence(
                new SequenceParallel (this.TextOn ("Nothing strange happens. They are watching TV......", canvasLight, bubbleTextL))
                );
    }

    protected Node TalktoLighter()
    {
        // You ask A to fix light bulb and he is angry
        return new Sequence(
                new SequenceParallel (this.TextOn ("You: What are you doing here?", canvasLight, bubbleTextL)),
                new SequenceParallel (this.TextOn ("B: The light seems broken, can you help me ask somebody to fix that?", canvasTV, bubbleTextT))
                );
    }

    protected Node Ending1()
    {
        // You ask A to fix light bulb and he is angry
        return new Sequence(
                new SequenceParallel (this.TextOn ("You: The light bulb is broken, can you help that guy fix it?", canvasTV, bubbleTextT)),
                new SequenceParallel (this.TextOn ("A: Hah? How dare you ask me to do that?", canvasLight, bubbleTextL))
                );
    }
    protected Node Ending2(GameObject partc)
    {
        Func<bool> act = () => (lamp.enabled);
        Node trigger = new DecoratorLoop (new LeafAssert (act));

        // You ask A to fix light bulb and he is angry
        return new Sequence(
                new SequenceParallel (this.TextOn ("You: The light bulb is broken, can you help that guy fix it?", canvasTV, bubbleTextT)),
                new SequenceParallel (this.TextOn ("C: Alright. I'll take a look at that.", canvasLight, bubbleTextL)),

                new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                        new SequenceParallel(
                            trigger,
                            new Sequence(
                                this.LightOff(partc.GetComponent<BehaviorMecanim>()), this.WatchTV(partc.GetComponent<BehaviorMecanim>(), TVp3, SofaIK3)))))
                );
    }


    protected Node Greeting(String Role)
    {
        return new SelectorShuffle(
                this.TextOn(Role + "Oh! Hey~", canvasTV, bubbleTextT),
                this.TextOn(Role + "They got some really nice TV show, right?",canvasTV, bubbleTextT),
                this.TextOn(Role + "Hmmmm.......",canvasTV, bubbleTextT),
                this.TextOn(Role + "How are you?",canvasTV, bubbleTextT),
                this.TextOn(Role + "Cool!",canvasTV, bubbleTextT),
                this.TextOn(Role + "Nice weather today, isn't it?",canvasTV, bubbleTextT)
                );
    }

    protected Node Simplify(GameObject parta, GameObject partb, GameObject partc)
    {
        // A ask B turn off light
        // A ask C turn on TV

        //Val<bool> pp = Val.V (() => lamp.enabled);
        Func<bool> act = () => (lamp.enabled);
        Node trigger = new DecoratorLoop (new LeafAssert (act));

        Func<bool> playerinRangeA = () => (parta.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeB = () => (partb.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeC = () => (partc.GetComponentInChildren<PlayerinRange>().playerinRange & (angryCount < 3));
        Func<RunStatus> switchinRange = () => (lightSwitch.GetComponentInChildren<PlayerinRange>().PlayerInRange()?RunStatus.Success:RunStatus.Running);
        Func<bool> clicked = () => (Player.GetComponentInChildren<PlayerController>().clicked);

        Node triggerA = new DecoratorLoop (new LeafAssert (playerinRangeA));
        Node triggerB = new DecoratorLoop (new LeafAssert (playerinRangeB));
        Node triggerC = new DecoratorLoop (new LeafAssert (playerinRangeC));
        Node triggerSwitch =new LeafInvoke (switchinRange);
        Node triggerClick = new DecoratorLoop (new LeafAssert (clicked));

        Func<bool> angry = () => (angryCount >= 3);
        Node triggerAngry = new DecoratorLoop (new LeafAssert (angry));

        Node trigger_preAngry = new LeafAssert (LightOffRole);

        return new SequenceParallel (
                new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                        new SequenceParallel(
                            triggerA,
                            new Sequence(
                                this.Greeting("A: ")
                                // this.TextOn("Hi! I'm A",canvasTV, bubbleTextT)
                                )
                            ))
                    ),
                new SequenceParallel (
                    new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                            new SequenceParallel(
                                triggerB,
                                new Sequence(
                                    this.Greeting("B: ")
                                    // this.TextOn("Hi! I'm B",canvasTV, bubbleTextT)
                                    )
                                ))
                        )),
                new SequenceParallel (
                    new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                            new SequenceParallel(
                                triggerC,
                                new Sequence(
                                    this.Greeting("C: ")
                                    // this.TextOn("Hi! I'm C",canvasTV, bubbleTextT)
                                    )
                                ))
                        )),
                new SequenceParallel (
                        new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                                new SequenceParallel(
                                    triggerAngry,
                                    new Sequence(
                                        // this.Greeting("C: ")
                                        this.TextOn("B: HEY!!!!!! STOP TOUCHING THE SWITCH!!!!!!",canvasTV, bubbleTextT)
                                        )
                                    ))
                            )),
                new SequenceParallel (

                    new DecoratorLoop (
                            new Sequence(
                                triggerSwitch,
                                new Sequence(this.LightOff(Player.GetComponent<BehaviorMecanim>()), new LeafInvoke(() => this.LightOffRole()))
                                // this.NPCLightOff()
                                )
                        )),
                    new SequenceParallel (
                            new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                                    new SequenceParallel(
                                        triggerClick,
                                        new Sequence(
                                            Player.GetComponent<BehaviorMecanim>().Node_GoTo(Player.GetComponentInChildren<PlayerController>().dest)
                                            )
                                        ))
                                ))
                    );
    }

    protected bool LightOffRole()
    {
        angryCount += 1;
        Debug.Log(angryCount);
        return true;
    }

    // protected Node NPCLightOff(GameObject partb)
    // {
    //     angryCount += 1;
    //     Debug.Log(angryCount);
    //     return new Sequence(this.LightOff(partb.GetComponent<BehaviorMecanim>()), this.WatchTV(partb.GetComponent<BehaviorMecanim>(), TVp3, SofaIK3));
    // }

    protected Node AssignRoles(GameObject parta, GameObject partb, GameObject partc)
    {
        // A ask B turn off light
        // A ask C turn on TV

        //Val<bool> pp = Val.V (() => lamp.enabled);
        Func<bool> act = () => (lamp.enabled);
        Node trigger = new DecoratorLoop (new LeafAssert (act));

        Func<bool> playerinRangeA = () => (parta.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeB = () => (partb.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeC = () => (partc.GetComponentInChildren<PlayerinRange>().playerinRange);

        Node triggerA = new DecoratorLoop (new LeafAssert (playerinRangeA));
        Node triggerB = new DecoratorLoop (new LeafAssert (playerinRangeB));
        Node triggerC = new DecoratorLoop (new LeafAssert (playerinRangeC));

        return new Sequence (
                new SequenceParallel (this.faceAndPoint (parta.GetComponent<BehaviorMecanim>(), partb, 2000), this.TextOn ("You turn off the light", canvasLight, bubbleTextL)),
                new SequenceParallel (this.faceAndPoint (parta.GetComponent<BehaviorMecanim>(), partc, 2000), this.TextOn ("You turn on the TV", canvasLight, bubbleTextL)),

                new SequenceParallel (
                    this.WatchTV(parta.GetComponent<BehaviorMecanim>(), TVp1, SofaIK1),
                    new Sequence(this.LightOff(partb.GetComponent<BehaviorMecanim>()), this.WatchTV(partb.GetComponent<BehaviorMecanim>(), TVp3, SofaIK3)),
                    new Sequence(this.TVOnOff(partc.GetComponent<BehaviorMecanim>()), this.WatchTV(partc.GetComponent<BehaviorMecanim>(), TVp2, SofaIK2)),
                    // new Sequence(this.TextOn ("GOGOGO...", canvasTV, bubbleTextT)),
                    new DecoratorLoop (new DecoratorForceStatus (RunStatus.Success,
                            new SequenceParallel(
                                trigger,
                                new Sequence(
                                    this.LightOff(partb.GetComponent<BehaviorMecanim>()),
                                    this.WatchTV(partb.GetComponent<BehaviorMecanim>(), TVp3, SofaIK3))
                                )
                            ))

                    )

                );
    }


    protected Node pointOthers(GameObject parta, GameObject partb, GameObject partc)
    {
        return new SelectorShuffle (
                this.AssignRoles(parta, partb, partc),
                this.AssignRoles(parta, partc, partb),
                this.AssignRoles(partb, partc, parta),
                this.AssignRoles(partb, parta, partc),
                this.AssignRoles(partc, parta, partb),
                this.AssignRoles(partc, partb, parta)
                );
    }


    protected Node BuildTreeRoot()
    {

        Val<Vector3> pos1 = Val.V (() => p1.position);
        Val<Vector3> pos2 = Val.V (() => p2.position);
        Val<Vector3> pos3 = Val.V (() => p3.position);

        //Val<Vector3> face = Val.V (() => participant3.transform.position);
        Node setup = new Sequence
            (

             new SequenceParallel (
                 part1.Node_GoTo(pos1),
                 part2.Node_GoTo(pos2),
                 part3.Node_GoTo(pos3)//,
                 //Player.GetComponent<BehaviorMecanim>().Node_GoTo(Player.transform.position)
                 ),

             new LeafWait(500),

             this.pointOthers(participant1, participant2, participant3)

            );




        Node root = new SelectorParallel(
                setup,
                this.Simplify(participant1, participant2, participant3)
                );


        Val<Vector3> face1 = Val.V(() => participant4.transform.position);
        Val<Vector3> face2 = Val.V(() => participant1.transform.position);

        //Node root = new Sequence(part1.Node_OrientTowards(face1), participant4.GetComponent<BehaviorMecanim>().Node_OrientTowards(face2));
        return root;
    }
}
