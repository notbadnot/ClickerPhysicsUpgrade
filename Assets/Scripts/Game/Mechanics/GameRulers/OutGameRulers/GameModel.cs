using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameModel
{
    public class Leader : IComparable<Leader>
    {
        private string myname;
        private int myscore;
        private int mytime;
        private string mydifficulty;
        public Leader(string name, int score, int time, string difficulty)
        {
            myname = name;
            myscore = score;
            mytime = time;
            mydifficulty = difficulty;

        }
        public string Name
        {
            get { return myname; }
        }
        public int Score
        {
            get { return myscore; }
        }
        public int Time
        {
            get { return mytime; }
        }
        public string Difficulty
        {
            get { return mydifficulty;}
        }
        public static bool operator>(Leader a, Leader b)
        {
            if (a.myscore > b.myscore)
            {
                return true;
            }else { return false; }
        }
        public static bool operator <(Leader a, Leader b)
        {
            if (a.myscore < b.myscore)
            {
                return true;
            }
            else { return false; }
        }
        public int CompareTo (Leader other)
        {
            return (other.Score - this.Score);
        }
    }
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public Difficulty difficulty { get; set; } = Difficulty.Medium;
    public List<Leader> Leaders = new List<Leader>();

    public bool enableCrowding = true;



}
