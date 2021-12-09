using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class ProjBehaviorTree : MonoBehaviour
{

    public Light bulb;

    public GameObject player3;
    public GameObject player2;
    public GameObject player1;
    public GameObject player4;

    public Transform lightStand;
    public GameObject lSwitch1;
    public InteractionObject lSwitch2;
    public Transform TVSwitchStand;
    public GameObject TVSwitch1;
    public InteractionObject TVSwitch2;

    public FullBodyBipedEffector hand;

    public Transform TVStand;
    public Transform TVPosition;

    public Transform TVPos1;
    public Transform TVPos2;
    public Transform TVPos3;

    public InteractionObject Seat1;
    public InteractionObject Seat2;
    public InteractionObject Seat3;

    public FullBodyBipedEffector butt;

    public GameObject allLight;
    public GameObject SamsungNeoQLED;
    public Text TextLeft;
    public Text TextTop;

    public Transform point1;
    public Transform point2;
    public Transform point3;

    public GameObject ball;


    BehaviorMecanim part1;
    BehaviorMecanim part2;
    BehaviorMecanim part3;
    public GameObject Player;

    private int angryCount = 0;



    private BehaviorAgent behaviorAgent;

    void Start()
    {

        part1 = player3.GetComponent<BehaviorMecanim>();
        part2 = player2.GetComponent<BehaviorMecanim>();
        part3 = player1.GetComponent<BehaviorMecanim>();

        allLight.GetComponent<CanvasGroup>().alpha = 0;
        SamsungNeoQLED.GetComponent<CanvasGroup>().alpha = 0;
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();

        Player = GameObject.Find("Player");

    }


    protected Node faceAndPoint(BehaviorMecanim section, GameObject facing, int t)
    {
        Val<Vector3> face = Val.V(() => facing.transform.position);
        return new Sequence
            (
             section.Node_OrientTowards(face),
             section.Node_HandAnimation("looking", true),
             new LeafWait(t),
             section.Node_HandAnimation("looking", false)
            );
    }


    protected Node LightOff(BehaviorMecanim section)
    {
        Val<Vector3> pos = Val.V(() => lightStand.position);
        Val<Vector3> glance = Val.V(() => lSwitch1.transform.position);
        return new Sequence
            (
             section.Node_StopInteraction(butt),
             section.Node_GoTo(pos),
             section.Node_OrientTowards(glance),

             section.Node_StartInteraction(hand, lSwitch2),
             new LeafWait(500),
             new LeafInvoke(() => bulb.enabled = !bulb.enabled),

             section.Node_StopInteraction(hand)
            );
    }


    protected Node TVOnOff(BehaviorMecanim section)
    {
        Val<Vector3> pos = Val.V(() => TVSwitchStand.position);
        Val<Vector3> glance = Val.V(() => TVSwitch1.transform.position);
        return new Sequence
            (
             section.Node_GoTo(pos),
             section.Node_OrientTowards(glance),

             section.Node_StartInteraction(hand, TVSwitch2),
             new LeafWait(500),

             section.Node_StopInteraction(hand)
            );
    }

    protected Node TextOn(String s, GameObject c, Text te)
    {
        return new Sequence
            (
             new LeafInvoke(() => te.text = s),
             new LeafInvoke(() => c.GetComponent<CanvasGroup>().alpha = 1),
             new LeafWait(2000),
             new LeafInvoke(() => c.GetComponent<CanvasGroup>().alpha = 0)
            );

    }

    protected Node WatchTV(BehaviorMecanim pt, Transform p, InteractionObject s)
    {
        Val<Vector3> tpos = Val.V(() => TVPosition.position);
        Val<Vector3> standpos = Val.V(() => p.position);
        return new Sequence(
                pt.Node_GoTo(standpos),
                pt.Node_OrientTowards(tpos),
                pt.Node_StartInteraction(butt, s)
                );
    }

    private void updatePos(Val<Vector3> v, GameObject p)
    {
        p.GetComponent<SteeringController>().Target = v.Value;
    }

    protected Node StoryPause()
    {
        return new Sequence(
                new SequenceParallel(this.TextOn("All of them are watching Samsung Neo QLED", allLight, TextLeft))
                );
    }

    protected Node TalktoLighter()
    {
        return new Sequence(
                new SequenceParallel(this.TextOn("You: How you doing", allLight, TextLeft)),
                new SequenceParallel(this.TextOn("B: Bulbs seems to be broken, could you help me ask help from someone?", SamsungNeoQLED, TextTop))
                );
    }

    protected Node Ending1()
    {
        return new Sequence(
                new SequenceParallel(this.TextOn("You: Bulbs seems to be broken, could you help me fix?", SamsungNeoQLED, TextTop)),
                new SequenceParallel(this.TextOn("A: How did you even dare to ask this to me?", allLight, TextLeft))
                );
    }
    protected Node Ending2(GameObject partc)
    {
        Func<bool> at = () => (bulb.enabled);
        Node t = new DecoratorLoop(new LeafAssert(at));

        return new Sequence(
                new SequenceParallel(this.TextOn("You: The Bulbs seems to be broken, could you help the guy fix it?", SamsungNeoQLED, TextTop)),
                new SequenceParallel(this.TextOn("C:Sure, will do!!", allLight, TextLeft)),

                new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                        new SequenceParallel(
                            t,
                            new Sequence(
                                this.LightOff(partc.GetComponent<BehaviorMecanim>()), this.WatchTV(partc.GetComponent<BehaviorMecanim>(), TVPos3, Seat3)))))
                );
    }


    protected Node Greeting(String Role)
    {
        return new SelectorShuffle(
                this.TextOn(Role + "Oh! Hey~", SamsungNeoQLED, TextTop),
                this.TextOn(Role + "got some really nice show here?", SamsungNeoQLED, TextTop),
                this.TextOn(Role + "hmm.", SamsungNeoQLED, TextTop),
                this.TextOn(Role + "hru?", SamsungNeoQLED, TextTop),
                this.TextOn(Role + "sure thing", SamsungNeoQLED, TextTop),
                this.TextOn(Role + "cool weather, isnt it?", SamsungNeoQLED, TextTop)
                );
    }

    protected Node Simplify(GameObject parta, GameObject partb, GameObject partc)
    {

        Func<bool> act = () => (bulb.enabled);
        Node trigger = new DecoratorLoop(new LeafAssert(act));

        Func<bool> playerinRangeA = () => (parta.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeB = () => (partb.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeC = () => (partc.GetComponentInChildren<PlayerinRange>().playerinRange & (angryCount < 3));
        Func<RunStatus> switchinRange = () => (lSwitch1.GetComponentInChildren<PlayerinRange>().PlayerInRange() ? RunStatus.Success : RunStatus.Running);
        Func<bool> clicked = () => (Player.GetComponentInChildren<PlayerController>().clicked);

        Node triggerA = new DecoratorLoop(new LeafAssert(playerinRangeA));
        Node triggerB = new DecoratorLoop(new LeafAssert(playerinRangeB));
        Node triggerC = new DecoratorLoop(new LeafAssert(playerinRangeC));
        Node triggerSwitch = new LeafInvoke(switchinRange);
        Node triggerClick = new DecoratorLoop(new LeafAssert(clicked));

        Func<bool> angry = () => (angryCount >= 3);
        Node triggerAngry = new DecoratorLoop(new LeafAssert(angry));

        Node trigger_preAngry = new LeafAssert(LightOffRole);

        return new SequenceParallel(
                new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                        new SequenceParallel(
                            triggerA,
                            new Sequence(
                                this.Greeting("A: ")

                                )
                            ))
                    ),
                new SequenceParallel(
                    new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                            new SequenceParallel(
                                triggerB,
                                new Sequence(
                                    this.Greeting("B: ")

                                    )
                                ))
                        )),
                new SequenceParallel(
                    new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                            new SequenceParallel(
                                triggerC,
                                new Sequence(
                                    this.Greeting("C: ")

                                    )
                                ))
                        )),
                new SequenceParallel(
                        new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                                new SequenceParallel(
                                    triggerAngry,
                                    new Sequence(
                                        this.TextOn("B: Stop , touching the switch pls!", SamsungNeoQLED, TextTop)
                                        )
                                    ))
                            )),
                new SequenceParallel(

                    new DecoratorLoop(
                            new Sequence(
                                triggerSwitch,
                                new Sequence(this.LightOff(Player.GetComponent<BehaviorMecanim>()), new LeafInvoke(() => this.LightOffRole()))
                                )
                        )),
                    new SequenceParallel(
                            new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
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


    protected Node AssignRoles(GameObject parta, GameObject partb, GameObject partc)
    {

        Func<bool> act = () => (bulb.enabled);
        Node trigger = new DecoratorLoop(new LeafAssert(act));

        Func<bool> playerinRangeA = () => (parta.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeB = () => (partb.GetComponentInChildren<PlayerinRange>().playerinRange);
        Func<bool> playerinRangeC = () => (partc.GetComponentInChildren<PlayerinRange>().playerinRange);

        Node triggerA = new DecoratorLoop(new LeafAssert(playerinRangeA));
        Node triggerB = new DecoratorLoop(new LeafAssert(playerinRangeB));
        Node triggerC = new DecoratorLoop(new LeafAssert(playerinRangeC));

        return new Sequence(
                new SequenceParallel(this.faceAndPoint(parta.GetComponent<BehaviorMecanim>(), partb, 2000), this.TextOn("You turn off the light", allLight, TextLeft)),
                new SequenceParallel(this.faceAndPoint(parta.GetComponent<BehaviorMecanim>(), partc, 2000), this.TextOn("You turn on the TV", allLight, TextLeft)),

                new SequenceParallel(
                    this.WatchTV(parta.GetComponent<BehaviorMecanim>(), TVPos1, Seat1),
                    new Sequence(this.LightOff(partb.GetComponent<BehaviorMecanim>()), this.WatchTV(partb.GetComponent<BehaviorMecanim>(), TVPos3, Seat3)),
                    new Sequence(this.TVOnOff(partc.GetComponent<BehaviorMecanim>()), this.WatchTV(partc.GetComponent<BehaviorMecanim>(), TVPos2, Seat2)),

                    new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                            new SequenceParallel(
                                trigger,
                                new Sequence(
                                    this.LightOff(partb.GetComponent<BehaviorMecanim>()),
                                    this.WatchTV(partb.GetComponent<BehaviorMecanim>(), TVPos3, Seat3))
                                )
                            ))

                    )

                );
    }


    protected Node pointOthers(GameObject parta, GameObject partb, GameObject partc)
    {
        return new SelectorShuffle(
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

        Val<Vector3> pos1 = Val.V(() => point1.position);
        Val<Vector3> pos2 = Val.V(() => point2.position);
        Val<Vector3> pos3 = Val.V(() => point3.position);

        //Val<Vector3> face = Val.V (() => player1.transform.position);
        Node setup = new Sequence
            (

             new SequenceParallel(
                 part1.Node_GoTo(pos1),
                 part2.Node_GoTo(pos2),
                 part3.Node_GoTo(pos3)//,
                                      //Player.GetComponent<BehaviorMecanim>().Node_GoTo(Player.transform.position)
                 ),

             new LeafWait(500),

             this.pointOthers(player3, player2, player1)

            );




        Node root = new SelectorParallel(
                setup,
                this.Simplify(player3, player2, player1)
                );


        Val<Vector3> face1 = Val.V(() => player4.transform.position);
        Val<Vector3> face2 = Val.V(() => player3.transform.position);

        //Node root = new Sequence(part1.Node_OrientTowards(face1), player4.GetComponent<BehaviorMecanim>().Node_OrientTowards(face2));
        return root;
    }
}
