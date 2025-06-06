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

    public static Board GameBoard = new Board();
    public static List<Player> Players = new List<Player>();
    public static List<Player> Rank = new List<Player>();
    public static Player CurrentPlayer;
    
    //GAME START SCREEN
    /*
     REFERENCES: 
        - Console title: https://learn.microsoft.com/en-us/dotnet/api/system.console.title?view=net-9.0
        - Console Foreground color: https://learn.microsoft.com/en-us/dotnet/api/system.console.foregroundcolor?view=net-9.0
     */
    public static int GAMESTARTSCREEN(bool playAgain = false)
    {
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
        if (Rank.Count > 0) {            
            Console.WriteLine(@"                            POWER RANK                               ");
            Console.WriteLine(@"=====================================================================");
            foreach (Player player in Rank) {
                Console.WriteLine(player.Wins + " WINS of ("+player.GamesPlayed+") | " + player.Name); // CHANGE FOR THE TOSTRING METHOD
            }
            Console.WriteLine(@"=====================================================================");
        }
        if (playAgain)
        {
            Console.WriteLine(@"[0] PLAY AGAIN                                                       ");
        }
        else
        {
            Console.WriteLine(@"[1] HUMAN vs COMPUTER                                                ");
            Console.WriteLine(@"[2] HUMAN vs HUMAN                                                   ");
            Console.WriteLine(@"[3] EXIT                                                             ");

        }
        Console.WriteLine(@"=====================================================================");
        Console.Write("Enter your choice: ");
        int option = 0;
        
        option = int.Parse(s: Console.ReadLine());
        if (option < 1 || option > 3)
        {
            GAMESTARTSCREEN();
        }

        return option;
    }
    //SET PLAYERS
    public static void SetPlayers(int gameType) {
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
                    Console.Write(CurrentPlayer.Name + " - Drop coin on column:");
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
        GameReset();
    }

    //check if is there a winner
    //hypotesis will check with a simulated board if there will be a winner
    public static bool GameWin(bool hypotesis = false) 
    {                
        bool isThereWinner = false;
        for (int row = 5; row > 1; row--) {
            for (int col = 0; col < 7; col++) {
                if (GameBoard.Spots[row, col] == CurrentPlayer.Symbol) {

                    //vertical
                    if (GameBoard.Spots[row, col] == GameBoard.Spots[row - 1, col] && GameBoard.Spots[row, col] == GameBoard.Spots[row - 2, col]) {
                        isThereWinner = true;
                    };

                    //horizontal
                    if (col < 5) {                        
                        if (GameBoard.Spots[row, col] == GameBoard.Spots[row, col + 1] && GameBoard.Spots[row, col] == GameBoard.Spots[row, col + 2])
                        {
                            isThereWinner = true;
                        }                        
                    };
                    
                    //Diganoals from the midle
                    if (col > 0 && col < 6 && row < 5) {                        
                        if (GameBoard.Spots[row, col] == GameBoard.Spots[row + 1, col+1] && GameBoard.Spots[row, col] == GameBoard.Spots[row - 1, col - 1])
                        {
                            isThereWinner = true;
                        }
                        if (GameBoard.Spots[row, col] == GameBoard.Spots[row + 1, col - 1] && GameBoard.Spots[row, col] == GameBoard.Spots[row - 1, col + 1])
                        {
                            isThereWinner = true;
                        }
                    }
                }
            } 
        }
        if (isThereWinner && !hypotesis) {
            Console.WriteLine(CurrentPlayer.Name + " is the Winner!!!");
            //just to keep the screen showing the last play
            string playAgain = Console.ReadLine();
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
        Console.WriteLine("Tie");
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
        Spots = new char[6,7];
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                Spots[row, col] = '-';                
            }
        }
    }

    //FillSpot
    public bool FillSpot(int colPick, char symbol) {

        if (colPick < 1 || colPick > 7)
        {
            Console.WriteLine("Please pick a column between 1 and 7");
            return false;
        }

        int col = colPick - 1;        

        int row = 5;
        while (Spots[row, col] != '-' && row > 0) {
            row--;
        }

        if (Spots[row, col] != '-')
        {
            Console.WriteLine("Spot unavailable!");
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
        string displayBoard = "";
        for(int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {                
                displayBoard += Spots[row, col];                
                if (col == 6) {
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
    public abstract bool Play(int colDroped, Player player);
}
//CLASS PLAYER HUMAN
public class Human : Player {     
    //Play()
    public override bool Play(int colDroped, Player player) {
        Console.WriteLine("Human");
        return Game.GameBoard.FillSpot(colDroped, player.Symbol);
    }
    //ToString - Return name, game wins and games played - Freddy
}
//CLASS PLAYER COMPUTER
public class Computer: Player
{    
    //Play() - in computer case colDroped always = 0
    public override bool Play(int colDroped, Player aiPlayer)
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
        //9 - PLACE COIN IN ANY LOWER AVAILABLE SPOT
        if (colToDrop == -1)
            colToDrop = FirstAvailable();
                
        Game.GameBoard.FillSpot(colToDrop, aiPlayer.Symbol);
        return true;
    }

    private int BestSpot() {
        int MaxChancesToWin = 0;
        int colWithMaxChance = -1;
        for(int row = 5; row >= 0; row--) {
            for (int col = 0; col < 7; col++) {
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
                        colWithMaxChance = col;
                    }
                }
            }
        }
        return colWithMaxChance;
    }

    //FUNCTION TO CHECK THE POSSIBILITY OF A WIN IN A HYPOTETICAL FUTURE PLAY
    private bool CheckSpot(int row, int col) {
        bool isThereAWin = false;
        if (row > -1 && row < 6 && col > -1 && col < 7) {
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
        for (int row = 5; row >= 0; row--)
        {
            for (int col = 0; col < 7; col++)
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

        for (int row = 5; row >= 0; row--)
        {
            for (int col = 0; col < 7; col++)
            {
                if (Game.GameBoard.Spots[row, col] == '-')
                {
                    if (row < 5)
                    {
                        if (Game.GameBoard.Spots[row + 1, col] == '-')
                            continue;
                    }
                    if (CheckSpot(row, col))
                        return col+1;                    
                }
            }
        }
        Game.CurrentPlayer = this;
        return -1;
    }
    
    //check the first not loosing play available
    private int FirstAvailable() {        
        for (int row = 5; row >= 0; row--) {
            for (int col = 0; col < 7; col++) {
                if (Game.GameBoard.Spots[row, col] == '-') {
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
        return -1;
    }

    //TOSTRING - Freddy

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
            action = Game.GAMESTARTSCREEN();
            switch (action)
            {
                case 1:
                case 2:
                    //GAME LOOP
                    //Players handle methods
                    Game.SetPlayers(action);
                    //Game play
                    Game.GamePlay();
                    //Game over screen - Mikyle
                    //Play again? - Mikyle                    
                    break;
                case 3:
                    //Exit
                    return;
            }
        } while (action != 3);                
    }
} 
