<?php

//ESA Anticheat Checker
//Author: David "zGrav" Samuel
//Date: 29th July 2013

$gamearray = array('Counter-Strike Global Offensive', 'Dota 2', 'Quake Live', 'Shootmania', 'Team Fortress 2');



echo("<form method='post' action='checker2.php'>");


echo("The username must be typed as shown on ESA.");

echo("<br>");

echo("Also works with Match ID only and username only.");

echo("<br>");

echo("<br>");


echo("<input type='text' placeholder='Username' id='user' name='user'/>");



echo("<br>");



echo("<br>");



echo("<select name='games'>");



foreach($gamearray as $game){

    echo ('<option value="'.$game.'">'.$game.'</option>');

}



echo("</select>");



echo("<br>");



echo("<br>");



echo("<input type='text' placeholder='Match ID' id='matchid' name='matchid'/>");



echo("<br>");



echo("<br>");


echo("<input type='submit' name='submit' value='Check!'>");


echo("</form>");


?>