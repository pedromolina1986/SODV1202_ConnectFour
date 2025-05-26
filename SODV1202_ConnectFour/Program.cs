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

//CLASS PLAYER

//CLASS PLAYER HUMAN

//CLASS PLAYER COMPUTER

/*
DESCRIPTION PRO COMPUTER PLAYER (AI)
1 - IS THERE ANY SPOT TO WIN?
2 - IS THERE ANY SPOT WHICH THE OTHER PLAYER WILL WIN?
3 - IS THERE ANY SPOT THAT LEAVES ME 3 POSSIBILITIES TO WIN?
4 - IS THERE ANY SPOT THAT LEAVES ME 2 POSSIBILITIES TO WIN?
5 - IS THERE ANY SPOT THAT LEAVES ME 1 POSSIBILITY TO WIN?
6 - IS THERE ANY SPOT THAT LEAVES THE OTHER PLAYER 3 POSSIBILITIES TO WIN?
7 - IS THERE ANY SPOT THAT LEAVES THE OTHER PLAYER 2 POSSIBILITIES TO WIN?
8 - IS THERE ANY SPOT THAT LEAVES THE OTHER PLAYER 1 POSSIBILITY TO WIN?
9 - PLACE COIN IN ANY LOWER AVAILABLE SPOT
 */

//CLASS GAME

//CLASS BOARD

public class Program
{
    private static void Main(string[] args)
    {
        //GAME LOOP
            //Start game screen
            //Players handle methods
            //Game play
            //Game over screen
            //Play again?
    }
} 
