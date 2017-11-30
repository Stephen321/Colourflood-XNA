//-----------------------------------------------------------------------------------------------------------------
//Author:
////Name: Stephen Ennis
//Login ID = C00181305
//Date Created: 5/2/2014
//-----------------------------------------------------------------------------------------------------------------
//Description:
//A game where you have to flood the board with the same type of animal or colour of square. There are highscore tables,
//sounds,music and menus to adjust settings. There are 4 different gamemodes.
//------------------------------------------------------------------------------------------------------------------
//Know bugs/issues
//Corrupt data error on closing the programming (rarely happens)

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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //fonts to display text differently
        SpriteFont font;
        SpriteFont bigFont;
        SpriteFont infoFont;
        Texture2D imageStrip;//the sprite sheet which contains all the images
        Texture2D animalsImages; //animal image strip - all images to display all the animals
        Texture2D squaresImages; // all images to display all the squares
        Texture2D animalBackgrounds; //background for the animal tiles
        Texture2D plus; //texture to display plus/minus button to increase or decrease the size of the board
        Texture2D startButton;
        Texture2D continueButton;
        Texture2D exitButton;
        Texture2D mainMenuButton;
        Texture2D optionsButton; //go to options
        Texture2D infoButton; //show instructions 
        Texture2D musicButton; //turn off/on music
        Texture2D soundsButton;  //turn off/on sounds
        Texture2D elephantImage, giraffeImage, background, infoBubble; //used to decorate the game
        Song menusMusic; //play this music when not playing the game (menus,instructions,options,gameover)
        Song gameMusic; //play this music when the game is being played
        SoundEffect click; //for clicking animaml buttons
        SoundEffect clickMenus; //for clicking menu buttons
        SoundEffect gameOver;  //gameover sound
        MouseState previousMouseState; //info about the state the mouse was in, in the previous frame
        KeyboardState previousKeyboardState; //info about the state of the keyboard in the previous frame
        Random rndNum = new Random();
        const int MaxSize = 20; //max rows and cols in the array
        const int LowestAnimals = 4; //lowest amount of rows and cols possible
        const int TotalAnimals = 7; //total amount of different animals that there are
        const int MaxTables = 17; //there are 16 tables. One for each possible side of board. 4 - 20.
        const int plusX = 125; //x postion of the plus/minus button doesnt change
        const int plusY = 200; //top y position of the button doesnt change
        const int PlusButtonWidth = 102, PlusButtonHeight = 101; //dimensions of the red plus/minus button 
        Animal[,] animals = new Animal[MaxSize, MaxSize]; //2d array of the animal objects
        AnimalButton[] animalButtons = new AnimalButton[TotalAnimals]; //an array containing a button for each animal
        string[] animalNames = new string[7] { "panda", "monkey", "elephant", "crocodile", "hippo", "lion", "giraffe" }; //string array of all the animal types
        HighScoreTable[] highScoreTables = new HighScoreTable[MaxTables]; //array of the highscore tables (1 for each size board)
        int curSize; //current amount of rows and cols in the array
        Rectangle plusRect; //rectangle for location of the plus part of the red button
        Rectangle minusRect; //rectangle for location of the minus part
        Rectangle startRect; //rectangle for the location of the start button
        Rectangle continueRect; //location of continue button
        Rectangle exitRect; //location of exit button
        Rectangle mainMenuRect;
        Rectangle optionsRect;
        Rectangle infoRect;
        Rectangle musicRect, soundsRect;
        Rectangle squareSampleRect, animalSampleRect; //sample of what the animal and square images look like (used as buttons)
        Rectangle backgroundRect; //rectangle of the playable are to fit the background image into
        Rectangle infoBubbleRect; //position of the info bubble
        int curTurn, maxTurn; //the current turn the player is on. The max turns for this size board.
        int curTime, maxTime; //the current time the player is on . The max time for this board
        int curFilled, maxFilled;// the current and max amount of filled tiles on this board.
        int secondTime; //a timer that will increment roughly 50 times a second. when it reaches 50 it will decrease curTime remaining

        const int Splash = 0, Game = 1, Gameover = 2, Options = 3; //different gamemodes
        const int ButtonWidth = 150, ButtonHeight = 60; //height and width of the menu buttons is the same
        const int SplashButtonsX = 60; //all the splash buttons have the same x positions
        int gameMode = Splash; //current gamemode
        bool canContinue; //can only continue the current game if the size of the board hasnt been changed 
        bool nameEntered = false; //has the player entered their name yet
        bool showInfo = false; //if the info screen should be shown or not
        bool playMusic; //play music if this is true otherwise dont
        bool playSounds; //play sounds if this is true otherwise dont
        int score; //the players score
        int countFilled = 0; //checks how many animals were filled in this click

        string playerName;//the name of the player which they will type in

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //sets up all variables for the first time
            this.IsMouseVisible = true; //display the mouse 
            previousMouseState = Mouse.GetState(); 
            previousKeyboardState = Keyboard.GetState();
            curSize = 6; //start with default rows and cols

            for (int i = 0; i < MaxSize; i++)
            {
                for (int j = 0; j < MaxSize; j++)
                {
                    animals[i, j] = new Animal(rndNum, curSize, curSize); //create new animals
                }
            }
            animals[0, 0].FloodFilled = true; //top left tile starts off as flood filled

            for (int i = 0; i < TotalAnimals; i++)
            {
                animalButtons[i] = new AnimalButton(i); //create new animal buttons for animal 0 - 6 ( 7 animals)
            }

            for (int i = 0; i < MaxTables; i++)
            {
                highScoreTables[i] = new HighScoreTable(); //create new highscore tables
            }

            //position and size of the buttons
            plusRect = new Rectangle(plusX, plusY, PlusButtonWidth, PlusButtonHeight / 2);
            minusRect = new Rectangle(plusX, plusY + (PlusButtonHeight / 2), PlusButtonWidth, PlusButtonHeight / 2);
            startRect = new Rectangle(SplashButtonsX, 90, ButtonWidth, ButtonHeight);
            continueRect = new Rectangle(SplashButtonsX, 150, ButtonWidth, ButtonHeight);
            exitRect = new Rectangle(SplashButtonsX, 330, ButtonWidth, ButtonHeight);
            optionsRect = new Rectangle(SplashButtonsX, 270, ButtonWidth, ButtonHeight);
            infoRect = new Rectangle(SplashButtonsX, 210, ButtonWidth, ButtonHeight);
            musicRect = new Rectangle(380, 300, ButtonWidth, ButtonHeight);
            soundsRect = new Rectangle(380, 350, ButtonWidth, ButtonHeight);
            squareSampleRect = new Rectangle(70, 355, 80, 80);
            animalSampleRect = new Rectangle(160, 355, 80, 80);
            infoBubbleRect = new Rectangle(20, 20, 730, 170); //pos and size of info bubble
            backgroundRect = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height); //rectangle for the background image to be displayed in
            canContinue = false;
            RestartGame(); //set up board for the first time so if the player clicks continue at the start instead of start then it wont crash
            playerName = "";

            //set up sounds
            playMusic = true;
            playSounds = true;
            MediaPlayer.IsRepeating = true;


            base.Initialize();
        }

        private void RestartGame()
        { //will restart the game board with new variables when changing the size of the board
            previousMouseState = Mouse.GetState();
            ResetBoard();
            score = 0; //reset score to 0
            animals[0, 0].FloodFilled = true; //make the first one true again otherwise it would remain false
            curFilled = 1; //there will always be at least 1 tile flood filled at the start of a game
            maxFilled = curSize * curSize; //get the total amount of tiles on the board
            curTurn = 1; //starting turn
            maxTurn = (curSize - 1) * 3; //the max amount of turns 

            maxTime = (maxTurn * 2);
            curTime = maxTime; //current time starts at the max turn.

            DisableButtons(); //set all buttons back to being false
            UpdateFill(animals[0, 0].CurrentAnimal); //if there were three of the same animals underneath the [0, 0] then all 4 will be flood filled at the start of the new game            
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        { //load all the content into the game
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //load fonts
            bigFont = Content.Load<SpriteFont>("Font1"); 
            font = Content.Load<SpriteFont>("font");
            infoFont = Content.Load<SpriteFont>("infoFont");

            //load images that are used to decorate the screens
            elephantImage = Content.Load<Texture2D>("elephantimage");
            giraffeImage = Content.Load<Texture2D>("giraffeImage");
            background = Content.Load<Texture2D>("background");
            infoBubble = Content.Load<Texture2D>("infobubble");
            
            //load textures
            animalsImages = Content.Load<Texture2D>("animals");
            squaresImages = Content.Load<Texture2D>("squares");
            imageStrip = animalsImages; //start images off as animals by default
            plus = Content.Load<Texture2D>("plus");
            animalBackgrounds = Content.Load<Texture2D>("animalBackgrounds");

            //load button textures
            startButton = Content.Load<Texture2D>("startbutton");
            continueButton = Content.Load<Texture2D>("continuebutton");
            exitButton = Content.Load<Texture2D>("exitbutton");
            mainMenuButton = Content.Load<Texture2D>("mainmenubutton");
            optionsButton = Content.Load<Texture2D>("optionsbutton");
            musicButton = Content.Load<Texture2D>("musicbutton");
            soundsButton = Content.Load<Texture2D>("soundsbutton");
            infoButton = Content.Load<Texture2D>("infobutton");

            //load sounds
            menusMusic = Content.Load<Song>("menus");
            gameMusic = Content.Load<Song>("game");
            click = Content.Load<SoundEffect>("click");
            clickMenus = Content.Load<SoundEffect>("clickMenus");
            gameOver = Content.Load<SoundEffect>("gameover");
            MediaPlayer.Play(menusMusic); //start with menu music

            for (int i = 0; i < TotalAnimals; i++)
            {
                animalButtons[i].LoadContent(Content, "highlight", animalNames); //load content for the buttons
            }

            for (int i = 0; i < MaxTables; i++)
            {
                highScoreTables[i].LoadContent(Content, "highscorebackground"); //load content for the highscoretables
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        { //updates everything in the game
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            switch (gameMode)
            {
                case Splash: //if the game mode is currently set to the splash mode
                    UpdateSplash(gameTime); //update the splash mode only
                    break;
                case Game:
                    UpdateGame(gameTime);
                    break;
                case Gameover:
                    UpdateGameOver(gameTime);
                    break;
                case Options:
                    UpdateOptions(gameTime);
                    break;
            }
            base.Update(gameTime);
        }
        private void DisableButtons()
        { //resets all buttons back to being false
            for (int i = 0; i < TotalAnimals; i++)
            {
                animalButtons[i].Enabled = false;
            }
        }

        private void UpdateSplash(GameTime gameTime)
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.P) && previousKeyboardState.IsKeyDown(Keys.P) && showInfo)
            {
                showInfo = false; //leave the information screen if the p key was pressed while inside it
            }

            CheckButtons();//check if buttons have being clicked
            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();
        }

        private void UpdateGameOver(GameTime gameTime)
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.P) && previousKeyboardState.IsKeyDown(Keys.P) && nameEntered)
            {
                gameMode = Splash; //return to the splash screen
            }

            if (nameEntered == false)
            {
                GetUserInput(); //get the name being entered by the player
            } //end while

            if ((!Keyboard.GetState().IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter) && nameEntered == false) || score == 0)
            { //when the player presses enter or they score 0
                if ((playerName == null || playerName == "") && score != 0)
                    playerName = "Default"; //if the player didnt enter anything and only if they havnt scored 0
                nameEntered = true; //name has now been entered
                highScoreTables[curSize - 4].Update(curTime, maxTurn - curTurn, playerName.ToLower(), score); //-4 from curSize to get which table it is
            }

            CheckButtons(); //check if any buttosn have being clicked
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        private void GetUserInput()
        { //checks what keys the user has pressed and add them onto playerName
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in pressedKeys)    //http://www.gamedev.net/topic/457783-xna-getting-text-from-keyboard/
            {
                if (previousKeyboardState.IsKeyUp(key)) //if this key was released previously but is now pressed
                {
                    if (key == Keys.Back && playerName.Length > 0) // overflows
                        playerName = playerName.Remove(playerName.Length - 1, 1); //delete the last char at the end of this string
                    else if (key >= Keys.A && key <= Keys.Z && playerName.Length < 8) //if the the key is a character
                    {
                        playerName += key.ToString(); //add the pressed keys to the string

                    }// end if else 
                } //end foreach
            }
        }

        private void UpdateGame(GameTime gameTime)
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.P) && previousKeyboardState.IsKeyDown(Keys.P))
            {
                gameMode = Splash;
                if (playMusic)
                    MediaPlayer.Play(menusMusic); //change music
            }

            canContinue = true; //can continue the game so enable the button
            IsGameOver(); //check if any of the gameover conditions have occured
            CheckButtons(); //check what buttons have being clicked 
            ChangeButtons();

            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();
        }

        private void UpdateOptions(GameTime gameTime)
        {
            if (!Keyboard.GetState().IsKeyDown(Keys.P) && previousKeyboardState.IsKeyDown(Keys.P))
            {
                gameMode = Splash;
            }

            CheckButtons();
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        private void IsGameOver()
        { //checks to see if the game has ended
            secondTime++; //increase the secondTimer
            if (secondTime == 50) //when this gets to 50 then roughly a second has passed
            {
                secondTime = 0; //reset the second timer
                curTime--; //decrease the remaining time 
            }
            if (curTime < 0 || curTurn > maxTurn) //gameover
            {
                if (score != 0)//even if the player ran out of turns/time they can still get a highscore as long as their score wasnt 0
                {
                    nameEntered = false;
                    playerName = ""; //reset the player name to an empty string
                }

                gameMode = Gameover; //game is over
                canContinue = false; //can no longer continue from this point
                //if music/sounds are enabled the play them
                if (playMusic)
                    MediaPlayer.Play(menusMusic);
                if (playSounds)
                    gameOver.Play();
            }
            else if (curFilled == maxFilled)
            {
                nameEntered = false;
                playerName = ""; //reset the player name to an empty string
                gameMode = Gameover;
                if (playSounds)
                    gameOver.Play();
                if (playMusic)
                    MediaPlayer.Play(menusMusic);
                canContinue = false;
            }
        }

        private void CheckButtons()
        { //check if buttons were clicked and do something if they are
            if (gameMode == Splash) //if in the splash screen
            {
                if (showInfo == false) //not in the info screen
                {
                    if (CheckButtonClicked(startRect))
                    {
                        gameMode = Game; //if the start button has been clicked then start the game and restart it with any changes
                        if (playMusic)
                            MediaPlayer.Play(gameMusic);
                        RestartGame(); //reset the board and other variables
                    }

                    else if (CheckButtonClicked(continueRect) && canContinue)
                    {
                        gameMode = Game; //if the continue button has been clicked then continue the game
                        if (playMusic)
                            MediaPlayer.Play(gameMusic); 
                    }

                    else if (CheckButtonClicked(optionsRect))
                    {
                        gameMode = Options;
                    }

                    else if (CheckButtonClicked(exitRect))
                    {
                        Exit(); //exit the game
                    }

                    else if (CheckButtonClicked(infoRect))
                    {
                        showInfo = true;
                    }
                }
                else  //in the info screen
                {
                    if (CheckButtonClicked(mainMenuRect)) //only main menu button can be clicked
                        showInfo = false;
                }
            }

            else if (gameMode == Game) //if the game is playing 
            {
                for (int i = 0; i < TotalAnimals; i++)
                {
                    if (animalButtons[i].CheckClicked(previousMouseState)) //check if any of the animal buttons have been clicked
                    {
                        if (playSounds) //if sounds can be played
                        {
                            if (imageStrip == animalsImages) //if the current set of images being used are the animals
                                animalButtons[i].Play(); //get the animal button to play its own sound 
                            else
                                click.Play(); //play the normal "click" sound
                        }
                        curTurn++; //increase the current turn
                        UpdateFill(i); //check and update the flood fill of any animals that have been changed 
                    } //end if 
                }

                if (CheckButtonClicked(mainMenuRect))
                {
                    gameMode = Splash; //go back to main menu
                    if (playMusic)
                        MediaPlayer.Play(menusMusic); //change music if music can be played
                }
            }

            else if (gameMode == Gameover)
            {
                if (CheckButtonClicked(mainMenuRect) && nameEntered) //clicked and name has been entered
                {
                    gameMode = Splash;
                }
            }

            else //in options screen
            {

                if (CheckButtonClicked(mainMenuRect))
                    gameMode = Splash;

                else if (CheckButtonClicked(animalSampleRect))
                    imageStrip = animalsImages; //change what images are being used ( the current "skin")

                else if (CheckButtonClicked(squareSampleRect))
                    imageStrip = squaresImages;

                else if (CheckButtonClicked(musicRect))
                {
                    playMusic = !playMusic; //toggle the music to the opposite of what it currently is. (if false it will now be set to true)

                    if (playMusic)
                        MediaPlayer.Play(menusMusic); //start music if playMusic is true
                    else
                        MediaPlayer.Stop(); //stop the music if playMusic is now false
                }

                else if (CheckButtonClicked(soundsRect))
                    playSounds = !playSounds; //toggle the sounds on or off

                else if (CheckButtonClicked(plusRect)) //if the plus button has been clicked.
                {
                    if (curSize + 1 <= MaxSize && curSize + 1 <= MaxSize) //only increase if by increase it does not go above the max size of the array
                    {
                        canContinue = false;
                        curSize++; //increase the size of the board
                        RestartGame(); //reset board and variables with new values
                    }
                }

                else if (CheckButtonClicked(minusRect))
                {
                    if (curSize - 1 >= LowestAnimals && curSize - 1 >= LowestAnimals) //if decreasing the rows, it wouldnt go below the 
                    {                                                                 //lowest amount that there can be
                        canContinue = false;
                        curSize--;
                        RestartGame();
                    }
                } //end else if 
            } //end else 
        }

        private bool CheckButtonClicked(Rectangle rect)
        { //check if the player has clicked the area of the screen of the rectangle passed in then return true
            if (rect.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)) &&
                previousMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (playSounds)
                    clickMenus.Play(); //this will play the clickMenus sound for all menu buttons ( animal buttons are checked from their own class)
                return true;
            }
            else
                return false;
        }

        private void ChangeButtons()
        {
            //Check to see what animal buttons should be enabled and call a method to change them to true or false
            for (int i = 0; i < curSize; i++)
            {
                for (int j = 0; j < curSize; j++)
                {
                    if (animals[i, j].FloodFilled)
                    {
                        animalButtons[animals[i, j].CurrentAnimal].Enabled = false; //the animal button which represents the current flood filled tiles is false
                        CheckTileEnabled(i - 1, j);  //check the surrounding tiles to see what animals buttons have to be true or false
                        CheckTileEnabled(i + 1, j);
                        CheckTileEnabled(i, j - 1);
                        CheckTileEnabled(i, j + 1);
                    }
                } //end inner for loop
            } //end outer for loop
        }

        private void CheckTileEnabled(int row, int col)
        { //checks what buttons need to be enabled depending on what animals are adjacent to the current flood filled tiles.
            if ((row >= 0 && row < curSize && col >= 0 && col < curSize) && animals[row, col].FloodFilled == false)
            {
                animalButtons[animals[row, col].CurrentAnimal].Enabled = true;
            }
        }
  
        private void UpdateFill(int animal)
        { //a method to check if any of the current flood filled tiles are next to other tiles of this type and if yes
          //then make them flood filled aswell.

            for (int i = 0; i < curSize; i++) 
            {
                for (int j = 0; j < curSize; j++)
                {
                    if (animals[i, j].FloodFilled) 
                    {
                        animals[i, j].ChangeAnimal(animal); //change the current flood filled tiles to the animal button clicked
                        CheckTile(i - 1, j, i, j, animal); //check all the the adjacent tiles to see if they need to be flood filled
                        CheckTile(i + 1, j, i, j, animal);
                        CheckTile(i, j - 1, i, j, animal);
                        CheckTile(i, j + 1, i, j, animal);
                    }
                }

                score += countFilled * (10 * countFilled); //increase the score by a greater amount if more tiles were filled in one go
                countFilled = 0;
            } //end outer for loop
        }

        private void CheckTile(int row, int col, int i, int j, int animal) //i and j is the current flood filled tile and row and col are being checked
        //check the adjacent tiles passed to it to see if they meet the conditons to be flood filled. Animal is the animal type to check for.
        {
            if ((row >= 0 && row < curSize && col >= 0 && col < curSize) && animals[row, col].FloodFilled == false &&
                animals[i, j].CurrentAnimal == animals[row, col].CurrentAnimal)
            {
                animals[row, col].FloodFilled = true;
                animals[row, col].ChangeAnimal(animal);
                curFilled++; //how many filled this game
                countFilled++;//how many filled in this click
                CheckTile(row - 1, col, i, j, animal); //check all the the adjacent tiles to these new flood filled tiles ******
                CheckTile(row + 1, col, i, j, animal);
                CheckTile(row, col - 1, i, j, animal);
                CheckTile(row, col + 1, i, j, animal);
            }
            //Check to see if the index is not out of bounds first
            //check if the animal above ( i -1) isnt flood filled, if it isnt when check if it has the same image as 
            //the current flood filled one. If it does then make it flood filled.
        }

        private void ResetBoard()
        { //modify the size and images for the new sized board
            for (int i = 0; i < curSize; i++)
            {
                for (int j = 0; j < curSize; j++)
                {
                    animals[i, j].Reset(rndNum, curSize); //reset the board 
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        { //draw all screens
            GraphicsDevice.Clear(Color.AntiqueWhite);
            spriteBatch.Begin();

            switch (gameMode)
            {
                case Splash: //if the current gamemode is set to splash 
                    DrawSplash(); //draw the splash screen
                    break;
                case Game:
                    DrawGame();
                    break;
                case Gameover:
                    DrawGameOver();
                    break;
                case Options:
                    DrawOptions();
                    break;

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGameOver()
        {
            mainMenuRect = new Rectangle(60, 350, ButtonWidth, ButtonHeight); //change the position of the main menu button for this screen
            spriteBatch.Draw(background, backgroundRect, Color.White); //draw the background 
            spriteBatch.Draw(elephantImage, new Vector2(460, 310), Color.White); 
 
            if (playerName != null) //error checking
                spriteBatch.DrawString(bigFont, "Name: " + playerName.ToLower(), new Vector2(70, 240), Color.SteelBlue); //display the players name
            spriteBatch.DrawString(bigFont, "Score: " + String.Format("{0:D4}", score), new Vector2(70, 280), Color.SteelBlue); //display the score the player got

            if (nameEntered == false) 
            {
                spriteBatch.Draw(mainMenuButton, mainMenuRect, Color.White * 0.5F); //the player cant click this button until they enter their name so display it at 0.5 alpha
                spriteBatch.DrawString(bigFont, "Please enter your name (Max: 8 letters): ", new Vector2(15, 200), Color.Black); //display instrcutons
                highScoreTables[curSize - 4].Draw(spriteBatch, font, curSize, bigFont); //draw the highscore table before it is update 
            }
            else
            {
                highScoreTables[curSize - 4].Draw(spriteBatch, font, curSize, bigFont); //draw the current highscore table
                spriteBatch.Draw(mainMenuButton, mainMenuRect, Color.White); //draw the full opaque button now as it can be clicked
            }         
        }

        private void DrawGame()
        {

            spriteBatch.Draw(background, backgroundRect, Color.White); //draw background
            spriteBatch.DrawString(bigFont, String.Format("{0:D3}", curTime) + " Remaining.", new Vector2(525, 145), Color.Red); //display time
            spriteBatch.DrawString(bigFont, "Turns:" + "\n" + curTurn + "/" + maxTurn, new Vector2(520, 90), Color.SteelBlue); //display turns
            spriteBatch.DrawString(bigFont, "Filled:" + "\n" + curFilled + "/" + maxFilled, new Vector2(615, 90), Color.SteelBlue); //display filled 
            spriteBatch.DrawString(bigFont, "Score:" + "\n" + String.Format("{0:D4}", score), new Vector2(565, 30), Color.SteelBlue); //display score 

            for (int i = 0; i < curSize; i++)
            {
                for (int j = 0; j < curSize; j++)
                {
                    animals[i, j].Draw(spriteBatch, i, j, imageStrip, animalBackgrounds); //draw all animals
                }
            }

            for (int i = 0; i < TotalAnimals; i++)
            {
                animalButtons[i].Draw(spriteBatch, imageStrip); //draw all the buttons.
            }

            mainMenuRect = new Rectangle(515, 390, ButtonWidth, ButtonHeight); //change the position of the main menu button for this screen
            spriteBatch.Draw(mainMenuButton, mainMenuRect, Color.White);
        }

        private void DrawSplash()
        {
            spriteBatch.Draw(background, backgroundRect, Color.White); //draw background
            if (showInfo)
            {
                spriteBatch.Draw(infoBubble, infoBubbleRect, Color.White); //draw info speech bubble which contains instructions
                spriteBatch.Draw(giraffeImage, new Vector2(130, 180), Color.White);
                mainMenuRect = new Rectangle(SplashButtonsX + 30, 390, ButtonWidth, ButtonHeight); //change the position of the main menu button for this screen
                spriteBatch.Draw(mainMenuButton, mainMenuRect, Color.White); //draw the main menu button on this screen
            }
            else //not in he info screen
            {
                spriteBatch.Draw(startButton, startRect, Color.White); 
                spriteBatch.Draw(optionsButton, optionsRect, Color.White);
                if (canContinue)
                    spriteBatch.Draw(continueButton, continueRect, Color.White); // if the game can be continued draw this button fully opaque
                else
                    spriteBatch.Draw(continueButton, continueRect, Color.White * 0.5F); //else draw it at 0.5 alpha
                //draw other buttons
                spriteBatch.Draw(exitButton, exitRect, Color.White);
                spriteBatch.Draw(infoButton, infoRect, Color.White);
            }
        }

        private void DrawOptions()
        {
            string onOrOff = ""; //string to show if music/sounds is off or on
            string skinString = ""; //string to show what skin is being used (animals or squares)
            float animalButtonAlpha, squareButtonAlpha; //these are used to change the opacity of the animal and square buttons in the options

            spriteBatch.Draw(background, backgroundRect, Color.White);//background
            if (imageStrip == animalsImages)
            {

                skinString = "Animals"; //Animal images are currently being displayed
                animalButtonAlpha = 0.5F; //the animal button will now be faded out as clicking it will have no effect
                squareButtonAlpha = 1.0F; //the square button will now be returned to normal as it can be clicked again
            }
            else
            {
                skinString = "Squares";
                animalButtonAlpha = 1.0F;
                squareButtonAlpha = 0.5F;
            }
            spriteBatch.DrawString(bigFont, "Current size: " + curSize, new Vector2(85, 170), Color.Black); //display current size 
            spriteBatch.DrawString(bigFont, "Skin: " + skinString, new Vector2(95, 325), Color.Black); //display current skin
            highScoreTables[curSize - 4].Draw(spriteBatch, font, curSize, bigFont); //draw the highscore table for the current size
            //draw tile buttons
            spriteBatch.Draw(animalsImages, animalSampleRect, new Rectangle(81, 81, 78, 78), Color.White * animalButtonAlpha); //the second rectangle on the image strip without the border outlines
            spriteBatch.Draw(squaresImages, squareSampleRect, new Rectangle(81, 81, 78, 78), Color.White * squareButtonAlpha); 
            //draw buttons
             mainMenuRect = new Rectangle(380, 400, ButtonWidth, ButtonHeight); //change the position of the main menu button for this screen
            spriteBatch.Draw(mainMenuButton, mainMenuRect, Color.White);
            spriteBatch.Draw(plus, new Vector2(plusX, plusY), Color.White);
            spriteBatch.Draw(musicButton, musicRect, Color.White);
            spriteBatch.Draw(soundsButton, soundsRect, Color.White);

            if (playMusic)
                onOrOff = "On"; //music can be played
            else
                onOrOff = "Off";
            spriteBatch.DrawString(bigFont, onOrOff, new Vector2(480, 317), Color.White);
            if (playSounds)
                onOrOff = "On"; //sounds can be played
            else
                onOrOff = "Off";
            spriteBatch.DrawString(bigFont, onOrOff, new Vector2(480, 367), Color.White);
        }
    }
}