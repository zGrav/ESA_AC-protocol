<?php

//ESA Anticheat Checker
//Author: David "zGrav" Samuel
//Date: 29th July 2013

error_reporting(0);

$user = preg_replace('/[^A-Z\\040a-z0-9_-]+/','-', $_REQUEST['user']);

$game = preg_replace('/[^A-Z\\040a-z0-9_-]+/','-', $_REQUEST['games']);

$matchid = $_REQUEST['matchid'];

$matchid = (integer)$matchid;

$userdir = "logs/" . $user . "/";

$gamedir = "logs/" . $user . "/" . $game . "/";

$matchdir = "logs/" . $user . "/" . $game . "/" . $matchid . "/";

$beforegametxt = "ac_logbeforegame_*";

$beforegamejpeg = "screen_beforegame_*";

$zipfile = "log_*";

echo("User: " . $user);

echo("<br>");

echo("Game: " . $game);

echo("<br>");

echo("Match ID: " . $matchid);

echo("<br>");

echo("<br>");

echo("Starting checker...");

echo("<br>");

echo("<br>");

echo("If the ZIP log does not show up, please try again later since it can be a large file.");

echo("<br>");

echo("If a substancial amount of time has passed, e.g 3+ hours, best chance is that the file didn't get uploaded :)");

echo("<br>");

echo("<br>");

if ((!$user == "") && ($matchid == 0)) {

$dircount = count(glob('logs/' . $user . '/*/*/'));

if ($dircount == 0) {
die("No results found.");
}

foreach(glob('logs/' . $user . '/*/*/') as $dir)   
{

$user2 = $dir;

$user2 = str_replace("logs/", "", $user2);

$user2 = str_replace("/" . $game . "/" . $matchid . "/", "", $user2);

echo "Directory: " . $dir . "<br />";

echo("<br>");

echo("<a href='http://esagamer.com/players/$user2'>User page on ESA</a>");

echo("<br>");
echo("<br>");

echo("Before game log:");

echo("<br>");

$filecount = count(glob($dir . $beforegametxt));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}


foreach (glob($dir . $beforegametxt) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("Before game screenshot:");

echo("<br>");

$filecount = count(glob($dir . $beforegamejpeg));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($dir . $beforegamejpeg) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("ZIP log:");

echo("<br>");

$filecount = count(glob($dir . $zipfile));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($dir . $zipfile) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("---------------------------------------------------");

echo("<br>");

echo("<br>");  
   
}
exit(0);   

}

if (($user == "") && ($matchid > 0)) {
$dircount = count(glob('logs/*/' . $game . '/' . $matchid . '/'));

if ($dircount == 0) {
die("No results found.");
}

foreach(glob('logs/*/' . $game . '/' . $matchid . '/') as $dir)   
{

$user2 = $dir;

$user2 = str_replace("logs/", "", $user2);

$user2 = str_replace("/" . $game . "/" . $matchid . "/", "", $user2);

echo "Directory: " . $dir . "<br />";

echo("<br>");

echo("<a href='http://esagamer.com/players/$user2'>User page on ESA</a>");

echo("<br>");
echo("<br>");

echo("Before game log:");

echo("<br>");

$filecount = count(glob($dir . $beforegametxt));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}


foreach (glob($dir . $beforegametxt) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("Before game screenshot:");

echo("<br>");

$filecount = count(glob($dir . $beforegamejpeg));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($dir . $beforegamejpeg) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("ZIP log:");

echo("<br>");

$filecount = count(glob($dir . $zipfile));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($dir . $zipfile) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("---------------------------------------------------");

echo("<br>");

echo("<br>");  
   
}
exit(0);   
}

if (!file_exists($userdir)) {
die('User not found!');
}

if (!file_exists($gamedir)) {
die('Game dir for user ' . $user . ' was not found!');
}

if (!file_exists($matchdir)) {
die('Match ' . $matchid . ' for user ' . $user . ' on game ' . $game . ' was not found!');
}

echo("Before game log:");

echo("<br>");

$filecount = count(glob($matchdir . $beforegametxt));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($matchdir . $beforegametxt) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("Before game screenshot:");

echo("<br>");

$filecount = count(glob($matchdir . $beforegamejpeg));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($matchdir . $beforegamejpeg) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

echo("ZIP log:");

echo("<br>");

$filecount = count(glob($matchdir . $zipfile));

if ($filecount == 0) {
echo("Not found.");
    echo("<br>");
    echo("<br>");
}

foreach (glob($matchdir . $zipfile) as $filefound) {
    echo("$filefound was found, size " . filesize($filefound));
    echo("<br>");
    echo("<br>");
}

?>