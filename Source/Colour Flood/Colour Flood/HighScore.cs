using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Colour_Flood
{
    class HighScore
    {
        string playerName; //the name of the player who got this score
        //inforamtion on how well the player did
        int turnsRemaining;
        int timeRemaining;
        int score;

        public HighScore()
        {
            //default values of 0 and ""
            turnsRemaining = 0;
            timeRemaining = 0;
            playerName = "";
            score = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteFont font)
        { //display string with info about the highscore
            spriteBatch.DrawString(font, "  Score: " + String.Format("{0:D4}", score) + "  Turns Remaining: " + String.Format("{0:D2}", turnsRemaining) +
                                    "  Time Remaining: " + String.Format("{0:D3}", timeRemaining) + "  Player:" + playerName, position, Color.Black);
        }

        public void Update(int theTimeRemaining, int theTurnsRemaining, string thePlayerName, int theScore)
        { //update this highscore with new info
            turnsRemaining = theTurnsRemaining;
            timeRemaining = theTimeRemaining;
            playerName = thePlayerName;
            score = theScore;
        }

        //properties
        public int Turns
        {
            get { return turnsRemaining; }
        }

        public int Time
        {
            get { return timeRemaining; }
        }

        public int Score
        {
            get { return score; }
        }

        public string PlayerName
        {
            set { playerName = value; }
        }
    }
}
