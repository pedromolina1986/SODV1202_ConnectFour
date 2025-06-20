/*
Final Project
GROUP: N
Students Names and IDs: Pedro Molina(467777)
Date: 2025-06-05
Course: SODV1202: Introduction to ObjectOriented Programming
Instructor: Michael Dorsey
*/

/*
REFERENCES:

COMPUTER PLAYER SIMULATION:

CHATGPT - PROMPT - What is the best way to create an algorithm to simulate an human player against another human player?

Human-like Simulation Approaches 

A. Rule-based / Heuristic AI 
  
Simulate human-like decisions with if-else rules.   
Encode common mistakes, risk-taking, hesitation, etc.  
Best for: Simple games, explainability, mimicking casual human behavior. 


B. Behavioral Cloning (Supervised Learning)
   
Train a model (e.g., neural network or decision tree) on past human games.  
Input: game state → Output: likely human move  
Best for: Realistic mimicry, especially with large human gameplay datasets.   


C. Reinforcement Learning with Human Bias
   
Train a model using reinforcement learning (RL) but:  
Penalize “non-human” behavior (e.g., perfect optimization)  
Encourage exploration, mistakes, or hesitation    
Add noise or constraints to make it less “superhuman”  
Best for: Learning strategies organically, with human-like imperfections   


D. Cognitive Models (e.g., ACT-R, SOAR)
   
Use cognitive architectures to model actual human decision-making processes.   
Simulate working memory, attention span, error rate.  
Best for: Psychology-based simulations or research   


E. Hybrid Models  
 
Combine heuristics + ML + randomness for more nuanced behavior.   
e.g., Use rules for common scenarios, but ML for strategy.
 
******OUR CONCLUSION******

OPTION A IS THE ONE THAT FITS BETTER FOR THIS GAME BECAUSE WHIS GAME IS SIMPLE.

 */

using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using System;
using System.Numerics;
using System.Collections.Generic;

//CLASS GAME - Descided to create as a static class because we do not need to instantiate it
//and it is immutable during this application giving global settings to other classes and main program
public static class Game{
    public static int Rows = 6;
    public static int Cols = 7;
    public static Board GameBoard = new Board();
    public static List<Player> Players = new List<Player>();
    public static List<Player> Rank = new List<Player>();
    public static Player? CurrentPlayer;

    //GAME START SCREEN
    /*
     REFERENCES: 
        - Console title: https://learn.microsoft.com/en-us/dotnet/api/system.console.title?view=net-9.0
        - Console Foreground color: https://learn.microsoft.com/en-us/dotnet/api/system.console.foregroundcolor?view=net-9.0
     */
    public static int GameStartScreen(bool playAgain = false)
    {

        if (playAgain)
        {
            CurrentPlayer = Players[0];
            GameBoard = new Board();                        
            GamePlay();
        }
        Console.Title = "Connect Four - Console Edition";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Clear();

        Console.WriteLine(@"               _____                          _     _   _            ");
        Console.WriteLine(@"              / ____|                        | |   | | | |           ");
        Console.WriteLine(@"             | |     ___  _ ___ _ ___ ___  __| |_  | |_| |           ");
        Console.WriteLine(@"             | |    / _ \| '_  | '_  | _ \/ _| __| |_ _  |           ");
        Console.WriteLine(@"             | |___| (_) | | | | | | | __/ (_| |_      | |           ");
        Console.WriteLine(@"              \_____\___/|_| |_|_| |_|___\___|\__|     |_|           ");
        Console.WriteLine(@"                                                                     ");
        Console.WriteLine(@"=====================================================================");
        Console.WriteLine(@"                   Welcome to Console Connect Four!                  ");
        Console.WriteLine(@"=====================================================================");
        if (Rank.Count > 0)
        {
            Console.WriteLine(@"                            POWER RANK                               ");
            Console.WriteLine(@"=====================================================================");
            foreach (Player player in Rank)
            {
                Console.WriteLine(player.Wins + " WINS of (" + player.GamesPlayed + ") | " + player); // CHANGE FOR THE TOSTRING METHOD
            }
            Console.WriteLine(@"=====================================================================");
        }
        Console.WriteLine(@"[1] HUMAN vs COMPUTER                                                ");
        Console.WriteLine(@"[2] HUMAN vs HUMAN                                                   ");
        Console.WriteLine(@"[3] EXIT                                                             ");
        Console.WriteLine(@"=====================================================================");
        Console.Write("Enter your choice: ");
        string optionString = "";
        int option = 0;
        do
        {
            optionString = Console.ReadLine();
            int.TryParse(optionString, out option);            
        } while (option != 1 && option != 2 && option != 3);     
        return option;
    }
    //SET PLAYERS
    public static bool SetPlayers(int gameType) {
        Human newPlayer1 = new Human();
        Console.Write("Player 1 - Type your name and press ENTER:");
        newPlayer1.Name = Console.ReadLine();
        newPlayer1.Symbol = 'O';
        Players.Add(newPlayer1);
        if (gameType == 1)
        {
            Computer newPlayer2 = new Computer();
            newPlayer2.Name = "COMPUTER";
            newPlayer2.Symbol = 'X';
            Players.Add(newPlayer2);
        }
        else 
        {
            Human newPlayer2 = new Human();
            Console.Write("Player 2 - Type your name and press ENTER:");
            newPlayer2.Name = Console.ReadLine();
            newPlayer2.Symbol = 'X';
            Players.Add(newPlayer2);
        }
        //Set current player
        CurrentPlayer = newPlayer1;

        foreach (Player player in Players) {
            if (string.IsNullOrEmpty(player.Name)) {                
                Players = new List<Player>();
                throw new GameExeception("Player name must not be empty! Try again!");
            }
        }

        return true;
    }

    //GAME PLAY
    public static void GamePlay()
    {
        Console.Write(GameBoard);
        string col;
        int intCol;
        bool endGame = false;
        do
        {
            do
            {
                if (CurrentPlayer is Human)
                {
                    Console.Write(CurrentPlayer.Name + "("+CurrentPlayer.Symbol+") - Drop coin on column:");
                    col = Console.ReadLine();
                    int.TryParse(col, out intCol);
                    //CHECK IF CAN PARSE TO INT
                }
                else {
                    intCol = 0;
                }
            } while (!CurrentPlayer.Play(intCol, CurrentPlayer));

            //check end game
            endGame = GameWin() || GameTie();

            if (!endGame) {
                int indCurrent = Players.FindIndex(f => f.Symbol == CurrentPlayer.Symbol);
                if (indCurrent == 1)
                {
                    CurrentPlayer = Players[0];
                }
                else
                {
                    CurrentPlayer = Players[1];
                }
            }            
        } while (!endGame);
        //after game play reset to the default settings        
        SetRank(CurrentPlayer);

        Console.WriteLine(CurrentPlayer.Name + " is the Winner!!!");
        //just to keep the screen showing the last play

        string playAgain;
        do
        {
            Console.WriteLine("Play Again? Yes (Y) or No (N):");
            playAgain = Console.ReadLine();
        } while (playAgain != "Y" && playAgain != "N");
        if (playAgain == "Y")
        {
            GameStartScreen(true);
        }
        else {
            GameReset();            
        }            
    }

    //check if is there a winner
    //hypotesis will check with a simulated board if there will be a winner
    public static bool GameWin(bool hypotesis = false) 
    {
        bool isThereWinner = false;

        for (int row = Game.Rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < Game.Cols; col++)
            {
                char symbol = GameBoard.Spots[row, col];
                if (symbol != CurrentPlayer.Symbol)
                    continue;

                // Vertical
                if (row - 3 >= 0)
                {
                    if (symbol == GameBoard.Spots[row - 1, col] &&
                        symbol == GameBoard.Spots[row - 2, col] &&
                        symbol == GameBoard.Spots[row - 3, col])
                    {
                        isThereWinner = true;
                    }
                }

                // Horizontal
                if (col + 3 < Game.Cols)
                {
                    if (symbol == GameBoard.Spots[row, col + 1] &&
                        symbol == GameBoard.Spots[row, col + 2] &&
                        symbol == GameBoard.Spots[row, col + 3])
                    {
                        isThereWinner = true;
                    }
                }

                // Diagonal Bottom-left to Top-right
                if (row - 3 >= 0 && col + 3 < Game.Cols)
                {
                    if (symbol == GameBoard.Spots[row - 1, col + 1] &&
                        symbol == GameBoard.Spots[row - 2, col + 2] &&
                        symbol == GameBoard.Spots[row - 3, col + 3])
                    {
                        isThereWinner = true;
                    }
                }

                // Diagonal Bottom-right to Top-left
                if (row - 3 >= 0 && col - 3 >= 0)
                {
                    if (symbol == GameBoard.Spots[row - 1, col - 1] &&
                        symbol == GameBoard.Spots[row - 2, col - 2] &&
                        symbol == GameBoard.Spots[row - 3, col - 3])
                    {
                        isThereWinner = true;
                    }
                }

                if (isThereWinner)
                {
                    break;
                }
            }

            if (isThereWinner)
            {
                break;
            }
        }        
        return isThereWinner;
    }

    //check game tie
    public static bool GameTie()
    {
        foreach (char spot in GameBoard.Spots) {
            if (spot == '-') {
                return false;
            }
        }
        Console.WriteLine("GAME TIE");
        string playAgain = Console.ReadLine();
        return true;
    }

    //SET RANK
    private static void SetRank(Player winner) {     
        //find winner
        int indexWinner = Rank.FindIndex(f => f.Name == winner.Name);
        if (indexWinner > -1)
        {
            Rank[indexWinner].GamesPlayed++;
            Rank[indexWinner].Wins++;
        }
        else 
        {
            winner.Wins++;
            winner.GamesPlayed++;
            Rank.Add(winner);
        }

        //find looser
        Player looser = Players.Find(f => f.Symbol != winner.Symbol);        
        int indexLooser = Rank.FindIndex(f => f.Name == looser.Name);
        if (indexLooser > -1)
        {
            Rank[indexLooser].GamesPlayed++;
        }
        else
        {
            looser.GamesPlayed++;
            Rank.Add(looser);
        }

        Rank.Sort((a, b) =>
        {
            //IComparable
            int result = b.Wins.CompareTo(a.Wins); // descending by Wins
            if (result == 0)
            {
                result = b.GamesPlayed.CompareTo(a.GamesPlayed); // descending by GamesPlayed
            }
            return result;
        });
    }

    //RESET GAME
    private static void GameReset() {
        //Clear list od players
        Players.Clear();
        //Create a new clean board
        GameBoard = new Board();
    }
}
//CLASS BOARD
public class Board {
    public char[,] Spots { get; set; }

    //Constructor    
    public Board() {        
        Spots = new char[Game.Rows,Game.Cols];
        for (int row = 0; row < Game.Rows; row++)
        {
            for (int col = 0; col < Game.Cols; col++)
            {
                Spots[row, col] = '-';                
            }
        }
    }

    //FillSpot
    public bool FillSpot(int colPick, char symbol) {

        if (colPick < 1 || colPick > Game.Cols)
        {
            Console.WriteLine("Please pick a column between 1 and " + Game.Cols);
            return false;
        }

        int col = colPick - 1;        

        int row = Game.Rows-1;
        while (Spots[row, col] != '-' && row > 0) {
            row--;
        }

        if (Spots[row, col] != '-')
        {
            Console.WriteLine("Spot unavailable! Press enter to play again!");
            Console.ReadLine();            
            Console.WriteLine(this);
            return false;
        }
        Spots[row, col] = symbol;
        Console.WriteLine(this);
        return true;
    }

    //Draw - ToString()
    public override string ToString()
    {
        Console.Clear();        
        string displayBoard = "";
        
        for (int col = 0; col < Game.Cols; col++) {
            displayBoard += " | " + (col+1) + " | ";
        }
        displayBoard += "\n";
        
        for (int col = 0; col < Game.Cols; col++)
        {
            displayBoard += " ----- ";
        }
        displayBoard += "\n";
        for (int row = 0; row < Game.Rows; row++)
        {
            for (int col = 0; col < Game.Cols; col++)
            {                
                displayBoard += " | " + Spots[row, col] + " | ";                
                if (col == Game.Cols-1) {
                    displayBoard += "\n";
                }
            }
        }        
        return displayBoard;
    }    
}
//CLASS PLAYER
public abstract class Player { 
    public int Wins { get; set; }
    public int GamesPlayed { get; set; }
    public char Symbol { get; set; }
    public string Name { get; set; }

    //Constructor
    //There is no need of a constructor in this abstract class

    //Play
    public abstract bool Play(int colDropped, Player player);

    public override string ToString()
    {
        return Name;
    }

}
//CLASS PLAYER HUMAN
public class Human : Player {     
    //Play()
    public override bool Play(int colDropped, Player player) {        
        return Game.GameBoard.FillSpot(colDropped, player.Symbol);
    }    
}
//CLASS PLAYER COMPUTER
public class Computer: Player
{    
    //Play() - in computer case colDroped always = 0
    public override bool Play(int colDropped, Player aiPlayer)
    {
        int colToDrop = -1;

        //DESCRIPTION PRO COMPUTER PLAYER (AI)
        //1 - IS THERE ANY SPOT TO WIN?
        colToDrop = SpotToWin();
        //2 - IS THERE ANY SPOT WHICH THE OTHER PLAYER WILL WIN?
        if (colToDrop == -1)
            colToDrop = SpotToLoose();

        //3 - IS THERE ANY SPOT THAT LEAVES ME 3 POSSIBILITIES TO WIN?            
        //4 - IS THERE ANY SPOT THAT LEAVES ME 2 POSSIBILITIES TO WIN?
        //5 - IS THERE ANY SPOT THAT LEAVES ME 1 POSSIBILITY TO WIN?
        if (colToDrop == -1)
           colToDrop = BestSpot();

        //6 - IS THERE ANY SPOT THAT LEAVES THE OTHER PLAYER 3 POSSIBILITIES TO WIN?
        //7 - IS THERE ANY SPOT THAT LEAVES THE OTHER PLAYER 2 POSSIBILITIES TO WIN?
        //8 - IS THERE ANY SPOT THAT LEAVES THE OTHER PLAYER 1 POSSIBILITY TO WIN?
        if (colToDrop == -1)
            colToDrop = BestSpot(true);//param true indicates that needs to change to the other player

        //9 - PLACE COIN IN ANY LOWER AVAILABLE SPOT
        if (colToDrop == -1)
            colToDrop = FirstAvailable();
                
        //fill the chosen spot
        Game.GameBoard.FillSpot(colToDrop, aiPlayer.Symbol);
        return true;
    }

    private int BestSpot(bool otherPlayer = false) {
        //if thee check is to loose
        int otherPlayerIndex = 0;
        if (otherPlayer) {
            otherPlayerIndex = Game.Players.FindIndex(f => f.Name != this.Name);
            Game.CurrentPlayer = Game.Players[otherPlayerIndex];
        }
        int MaxChancesToWin = 0;
        int colWithMaxChance = -1;
        //Since AI will check next col and next row inside the checking process we need to gor for length - 2, instead it will get a full row or column
        for(int row = Game.Rows-2; row >= 0; row--) {
            for (int col = 0; col < Game.Cols-2; col++) {
                //drop in the available spot
                if (Game.GameBoard.Spots[row, col] == '-') {
                    int chancesInThisSpot = 0;
                    Game.GameBoard.Spots[row, col] = Symbol;
                    //check around if there is a chance to win
                    //down-left                    
                    if (CheckSpot(row + 1, col - 1))
                        chancesInThisSpot++;
                    //left
                    if (CheckSpot(row, col - 1))
                        chancesInThisSpot++;
                    //up-left
                    if (CheckSpot(row - 1, col - 1))
                        chancesInThisSpot++;
                    //down-right
                    if (CheckSpot(row + 1, col + 1))
                        chancesInThisSpot++;
                    //right
                    if (CheckSpot(row, col + 1))
                        chancesInThisSpot++;
                    //up-right
                    if (CheckSpot(row - 1, col + 1))
                        chancesInThisSpot++;
                    //up
                    if (CheckSpot(row + 1, col))
                        chancesInThisSpot++;
                    //return the spot to orignal value
                    Game.GameBoard.Spots[row, col] = '-';

                    //if this spot has more chances of winnings spots then
                    if (MaxChancesToWin < chancesInThisSpot) {
                        MaxChancesToWin = chancesInThisSpot;
                        colWithMaxChance = col+1;
                    }
                }
            }
        }
        if (otherPlayer) {
            Game.CurrentPlayer = this;
        }        
        return colWithMaxChance;
    }

    //FUNCTION TO CHECK THE POSSIBILITY OF A WIN IN A HYPOTETICAL FUTURE PLAY
    private bool CheckSpot(int row, int col) {
        bool isThereAWin = false;
        if (row > -1 && row < Game.Rows && col > -1 && col < Game.Cols) {
            if (Game.GameBoard.Spots[row, col] == '-') {
                Game.GameBoard.Spots[row, col] = Game.CurrentPlayer.Symbol;
                isThereAWin = Game.GameWin(true);
                Game.GameBoard.Spots[row, col] = '-';
            }            
        }        
        return isThereAWin;
    }

    //check if it is a winning play
    private int SpotToWin() {
        for (int row = Game.Rows-1; row >= 0; row--)
        {
            for (int col = 0; col < Game.Cols; col++)
            {
                if (CheckSpot(row, col))
                    return col+1;                                                
            }
        }        
        return -1;
    }
    //check if it is a loosing play
    private int SpotToLoose()
    {
        //change the current player to simulate game winning        
        int otherPlayerIndex = Game.Players.FindIndex(f => f.Name != this.Name);
        Game.CurrentPlayer = Game.Players[otherPlayerIndex];

        for (int row = Game.Rows-1; row >= 0; row--)
        {
            for (int col = 0; col < Game.Cols; col++)
            {
                if (Game.GameBoard.Spots[row, col] == '-')
                {
                    if (row < Game.Rows-1)
                    {
                        if (Game.GameBoard.Spots[row + 1, col] == '-')
                            continue;
                    }
                    if (CheckSpot(row, col)) {
                        Game.CurrentPlayer = this;
                        return col + 1;
                    }                    
                }
            }
        }
        Game.CurrentPlayer = this;
        return -1;
    }
    
    //check the first not loosing play available
    private int FirstAvailable() {
        //in case of lost game get the fisrt found
        int firstFound = -1;
        for (int row = Game.Rows-1; row >= 0; row--) {
            for (int col = 0; col < Game.Cols; col++) {
                if (Game.GameBoard.Spots[row, col] == '-') {
                    if (firstFound < 0) {
                        firstFound = col;
                    }
                    //it needs to return +1 because the simulation of the human picking the column 
                    //AND it is how the mehtod FILLSPOT of the BOARD class works. Receiving the human
                    //column which is index+1

                    //after get the column check if there is any possibility to loose and picks the
                    //next spot until get one free spot
                    Game.GameBoard.Spots[row, col] = Symbol;
                    int spotToloose = SpotToLoose();
                    Game.GameBoard.Spots[row, col] = '-';
                    if (spotToloose == -1) {
                        return (col + 1);
                    }                    
                }
            }
        };
        return firstFound;
    }    

}
public class GameExeception : Exception {
    public GameExeception(string message) : base(message) { }    
}
//MAIN PROGRAM
public class ConnectFour
{
    private static void Main(string[] args)
    {        
        int action;        
        do
        {
            //Start game screen        
            action = Game.GameStartScreen();
            switch (action)
            {
                case 1:
                case 2:
                    bool gameSetted = false;
                    do
                    {
                        try
                        {
                            //GAME LOOP
                            //Players handle methods
                            gameSetted = Game.SetPlayers(action);                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }                        

                    } while (!gameSetted);
                    //Game play                                                    
                    Game.GamePlay();
                    break;
                case 3:
                    //Exit
                    return;
            }
        } while (action != 3);                
    }
} 
