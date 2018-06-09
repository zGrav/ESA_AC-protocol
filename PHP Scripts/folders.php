<?php
//ESA Anticheat PHP Folder Creator
//Author: David "zGrav" Samuel
//Date: 12th June 2013

error_reporting(0);

$user = preg_replace('/[^A-Z\\040a-z0-9_-]+/','-', $_GET['username']); //username
$game = preg_replace('/[^A-Z\\040a-z0-9_-]+/','-', $_GET['game']); //game folder
$matchid = $_GET['matchid']; // match id
$matchid = (integer)$matchid;

$switcharg = addslashes($_GET['arg']); //argument
$switcharg = (integer)$switcharg;

//Our argument switch that handles file/data parse.

if ($user == null) 
{
die('No username specified, PHP terminated.');
}

switch($switcharg) {

case 0: //user folder creation

mkdir("logs/" . $user, 0755);

echo('0');

break; 

case 1: //game folder creation

mkdir("logs/" . $user . "/" . $game, 0755);

echo('1');

break;

case 2: // match id folder creation

mkdir("logs/" . $user . "/" . $game . "/" . $matchid, 0755);

echo('2');

break;

default:

echo "Invalid argument detected.";

break;
}
//End of our switch

?>