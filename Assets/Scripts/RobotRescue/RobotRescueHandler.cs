using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RobotRescueHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rescueText;
    private int _rescueAmount = 0;
    private List<RobotRescue> _allRobots = new();
    public int PrepareRobot(RobotRescue robot)
    {
        _allRobots.Add(robot);
        return _allRobots.Count;
    }

    public void KillRobots()
    {
        foreach (RobotRescue robot in _allRobots)
        {
            robot.BringBack();
        }
        _allRobots = new();
    }

    public void GainRobots()
    {
        foreach (RobotRescue robot in _allRobots)
        {
            _rescueAmount++;
            Destroy(robot.gameObject);
        }
        _allRobots = new();
        rescueText.text = "Robots Rescued : " + _rescueAmount;
    }
}
