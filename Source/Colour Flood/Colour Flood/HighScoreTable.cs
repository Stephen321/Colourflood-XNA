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
    class HighScoreTable
    {
        const int MaxPlayers = 5; //max players per highscore table
        HighScore[] highscores = new HighScore[MaxPlayers]; //an array of the highscores in this table
        Texture2D highscoreBackground; //texture
        Vector2 position; //position of the highscore table

        public HighScoreTable()
        {
            for (int i = 0; i < MaxPlayers; i++)
                highscores[i] = new HighScore(); //create new highscore objects (5 per table)
            position = new Vector2(62, 45); //set position
        }

        public void Update(int theTimeRemaining, int theTurnsRemaining, string playerName, int score)
        { //update the highscore table
           
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (highscores[i].Score < score)
                {
                    for (int j = MaxPlayers - 1; j > i; j--) //push all the other highscores down one place
                    {  

                        // Save a reference to the object that will be written over
                        HighScore temp = highscores[j];
                        // Copy the reference from the one above j into j. (moving it down)
                        highscores[j] = highscores[j - 1];
                        // Now move the saved reference to j-1 (moving it up)
                        highscores[j - 1] = temp;
                    }
                    if (theTimeRemaining < 0)
                        theTimeRemaining = 0; //cant be less than 0

                    if (theTurnsRemaining < 0)
                        theTurnsRemaining = 0; //cant be less than 0

                    highscores[i].Update(theTimeRemaining, theTurnsRemaining, playerName, score); //update this highscore with new info
                    break;
                }
            }
        }

        public void LoadContent(ContentManager theContentManager, string assetName)
        {
            highscoreBackground = theContentManager.Load<Texture2D>(assetName);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, int curSize, SpriteFont bigFont)
        { //display highscore table
            position = new Vector2(62, 45); //reset poition back to original
            spriteBatch.Draw(highscoreBackground, position - new Vector2(60, 38), Color.White * 0.8F);
            spriteBatch.DrawString(bigFont, "HighScore table for the " + curSize + " x "+ curSize + " board:", position + new Vector2(175, -30), Color.Black); 
            for (int i = 0; i < MaxPlayers; i++)
            {
                spriteBatch.DrawString(font, "Rank:" + (i + 1), position - new Vector2(50, 0), Color.Black); 
                highscores[i].Draw(spriteBatch, position, font);
                position += new Vector2(0, 20); //change y position of where the string will be drawn 
            }
        }
    }
}
