<?php
// Connection details
$dbhost = "127.0.0.1:3307"; //!! localhost or the ip of the server 
$dbname = "xxx"; //!! name of the database
$dbuser = "xxx"; //!! username
$dbpass = "xxx"; //!! password

$mail = addslashes($_GET['email']);

mysql_connect($dbhost, $dbuser, $dbpass)or die("3");
$verb = mysql_select_db($dbname);

if (confirmuser($mail) == 1) {
die("Blocked.");
}

if ($mail == null) {
die("No email specified, script terminated.");
}

if ($verb)
{
    $sql = "SELECT username FROM users WHERE email_address='".$mail."'";
    $quer = mysql_query($sql) or die("4");
    $num = mysql_num_rows($quer);
    if ($num == 0)
    {
    echo("0");
    exit();
    }
    else
    {
    $row = mysql_fetch_object($quer);
    $getuser = $row->username;
    echo($getuser);
       exit();
    }
}

function confirmuser($value) { 
  
  $q = "SELECT attempts FROM loginattempts WHERE username = '$value'"; 

  $result = mysql_query($q); 
  $data = mysql_fetch_array($result); 

  if (!$data) { 
    return 0; 
  } 
  if ($data["attempts"] >= 3) 
  { 
      return 1; 
    } 
    else 
    { 
      return 0; 
    } 
   
  return 0; 
} 

mysql_close();
?>
