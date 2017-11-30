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
    class Animal
    {
        Rectangle posRect; //position of the animal on the strip
        const int MaxAnimals = 7;
        const int StripTileSize = 80;//the images on the strip keep the same width and height
        int tileSize;//size of the tiles to be drawn
        int curAnimal;
        const int TableSize = 480; //max size of the table which the animals are on
        bool floodFilled = false;
        const int Neutral = 0, Angry = 1, Shocked = 2; //the different animal expressions
        int expression; //which one to use

        public Animal(Random rndNum, int curRows, int curCols)
        {
            tileSize = TableSize / curRows; //default start screen
            curAnimal = rndNum.Next(0, MaxAnimals); //random animal
            expression = Angry; //start off with angry animals
            posRect = new Rectangle(curAnimal * StripTileSize, expression * StripTileSize, StripTileSize, StripTileSize); //position on the strip
        }

        public void Draw(SpriteBatch spriteBatch, int curRow, int curCol, Texture2D imageStrip, Texture2D animalBackgrounds)
        {
            Rectangle screenLocation = new Rectangle(tileSize * curRow, tileSize * curCol, tileSize, tileSize); //location on the screen
            //check what baackground to draw depending on what row and col this animal is
            if (curRow % 2 == 0)
            {
                if (curCol % 2 == 0) 
                {
                    spriteBatch.Draw(animalBackgrounds, screenLocation, new Rectangle(0, 0, 80, 80), Color.White); //draw the darker gold
                }
                else
                    spriteBatch.Draw(animalBackgrounds, screenLocation, new Rectangle(80, 0, 80, 80), Color.White); //draw the ligher gold
            }
            else
            {
                if (curCol % 2 == 0)
                {
                    spriteBatch.Draw(animalBackgrounds, screenLocation, new Rectangle(80, 0, 80, 80), Color.White); //draw the lighter gold
                }
                else
                    spriteBatch.Draw(animalBackgrounds, screenLocation, new Rectangle(0, 0, 80, 80), Color.White); //draw the darker gold
            }

            spriteBatch.Draw(imageStrip, screenLocation, posRect, Color.White);  //draw the animal itself
        }

        public void Reset(Random rndNum, int curRowsCols) 
        { //method to change the animal image to a random new one and the size depending on how many rows and cols there are
            curAnimal = rndNum.Next(0, MaxAnimals);
            tileSize = (TableSize / curRowsCols);
            expression = Angry; //reset expression to angry
            posRect = new Rectangle(curAnimal * StripTileSize, expression * StripTileSize, StripTileSize, StripTileSize); //get position on strip
            floodFilled = false; //no longer flood filled
        }

        public void ChangeAnimal(int newAnimal)
        {  //a method to change what animal to display 
            curAnimal = newAnimal;
            posRect = new Rectangle(curAnimal * StripTileSize, 0, StripTileSize, StripTileSize);
        }

        public void ChangeExpression()
        { //a method to change the expression to neutral when it is flood filled 
            if (floodFilled)
                expression = Neutral;
        }

        //properties
        public int CurrentAnimal
        {
            get { return curAnimal; } //return the current animal
            set { curAnimal = value; }
        }

        public bool FloodFilled
        {
            get { return floodFilled; } 
            set { floodFilled = value; }
        }
    }
}
