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
    class AnimalButton
    {
        
        Texture2D enabledImage; //image to show which button is enabled and clickable
        Rectangle posRect; //position of the correct animal on the image strip
        Rectangle location; //location of the button on the screen
        int animal; //animal represented by this button
        const int TileSize = 60; //tile width and height to be drawn on the screen
        const int StripTileSize = 80; //size of the tile on the imageStrip which contains the animal
        const int X = 700; //the x position to draw the buttons at which will never change.
        const int yOffSet = 30; // how much down off from the top of the screen they will be.
        bool enabled; //if the button can be clicked or not
        const int Angry = 1, Shocked = 2; //which animal expression to use
        int expression; //variable to hold current expression
        SoundEffect sound; //the animal sound to play when this button is clicked
        

        public AnimalButton(int theAnimal) 
        {
            animal = theAnimal;
            expression = Angry; //start off as angry
            posRect = new Rectangle(animal * StripTileSize, expression * StripTileSize, StripTileSize, StripTileSize); //position on strip
            location = new Rectangle(X, TileSize * animal + yOffSet, TileSize, TileSize); //position on screen
        }

        public void LoadContent(ContentManager theContentManager, string assetName, string[] names)
        {
            enabledImage = theContentManager.Load<Texture2D>(assetName);
            sound = theContentManager.Load<SoundEffect>(names[animal]); //load the sound in for the animal of this type
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D imageStrip)
        {
            posRect = new Rectangle(animal * StripTileSize, expression * StripTileSize, StripTileSize, StripTileSize); //expression might have changed
            if (enabled)  //if the button is clickable
            {
                spriteBatch.Draw(imageStrip, location, posRect, Color.White); //draw in off the left of the screen all at same x, different y depending on what animal it is
                spriteBatch.Draw(enabledImage, location, Color.White);
            }
            else
                spriteBatch.Draw(imageStrip, location, posRect, Color.White * 0.5f); //draw at 1/2 transparency if not enabled
        }
        public void Play()
        {
            sound.Play(); //this method will allow the animal button to play its own sound  when clicked
        }

        public bool CheckClicked(MouseState previousMouseState) 
        { //checked if the button has been clicked and is enabled, if it has then return true

            if (enabled && Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed &&
                location.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
            {
                expression = Shocked; //change to shocked while the mouse is being held over them
                return false;
            }
            if (enabled && Mouse.GetState().LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed &&
                location.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
            {
                expression = Angry;
                return true; //clicked
            }
            else
            {
                expression = Angry;
                return false; //hasnt been clicked
            }
        }

        public bool Enabled
        { 
            set { enabled = value; }
        }
    }
}
