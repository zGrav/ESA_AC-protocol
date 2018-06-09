<?php
//ESA Anticheat PHP Upload Script
//Author: David "zGrav" Samuel
//Date: 12th June 2013

$user = preg_replace('/[^A-Z\\040a-z0-9_-]+/','-', $_GET['user']);
$game = preg_replace('/[^A-Z\\040a-z0-9_-]+/','-', $_GET['game']);
$matchid = $_GET['matchid'];
$matchid = (integer)$matchid;

if ($user == null || $matchid == 0) {
die('Invalid.');
}

$uploaddir = "logs/" . $user . "/" . $game . "/" . $matchid . "/";
 
if (is_uploaded_file($_FILES['file']['tmp_name']))
{

  $uploadfile = $uploaddir . basename($_FILES['file']['name']);
 
$file_ext = strrchr($uploadfile, '.');

 $whitelist = array(".txt",".jpeg",".jpg",".zip"); 
 if (!(in_array($file_ext, $whitelist))) {
    die('Invalid extension.');
 }
 echo($matchid);
echo($uploaddir);
  echo "File ". $_FILES['file']['name'] . " uploaded successfully. \n";
  	
	if (file_exists($uploadfile)) 
  {
	$append = file_get_contents($_FILES['file']['tmp_name']);
	$do = fopen($uploadfile, 'a');
	
	fwrite($do, $append);
	
	fclose($do);
	
	echo "\n File appended succesful!";
  }
  
  else if (move_uploaded_file($_FILES['file']['tmp_name'], $uploadfile))
  {
    echo " \nFile upload succesful!";
  }
  else 
  {
    print_r($_FILES);
  }
}
else
{
  echo "Upload Failed!";
  print_r($_FILES);
}
?>