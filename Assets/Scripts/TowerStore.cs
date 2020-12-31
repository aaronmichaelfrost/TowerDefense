using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerStore : MonoBehaviour
{
    private bool displayOpen = false;


    public void OpenDisplay()
    {
        displayOpen = true;

        // Open the display UI

        // Generate a web of tower trees


        /*
         * To generate the tower unlock tree
         * 
            1. create array of unlocked after nodes under parent node, (unless child has already been drawn ANYWHERE else in the tree) 
                 starting at the closest empty position from the x pos of the parent ( recursive?), and equally spaced, add this array to the 
                 parent array which is like the container of all the tower types in the tree
            2. calculate average x of all unlocked and locked nodes in the parent level
      
            3. translate all nodes in this new level to the left by the differnece between the average of parents, and the average of this new level

            4. foreach connection, find it in the tree, and draw a line to it

            if a node is selected, then grey out all lines, and highlight the lines to unlockable or locked connected children that exist in the tree

            Uprades: tiered: Range, Damage Boost, Rate Of Fire Boost, not tiered: Element



            // Note that the skill tree is for different types of towers aka homing / sniper and not different upgrades
            
        // Upgrades cost gold and can be purchased for existing towers. there can be tiers of upgrades which just cost differet amounts of gold


         * */


    }

    public void Update()
    {
        if (displayOpen)
            UpdateDisplay();
    }



    private void UpdateDisplay()
    {
        // Update the tower store
        // - 


    }


    public void CloseDisplay()
    {
        // Do stuff to close the display



    }


}
