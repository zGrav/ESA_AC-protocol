<?php
// Connection details
$dbhost = "127.0.0.1:3307"; //!! localhost or the ip of the server 
$dbname = "xxx"; //!! name of the database
$dbuser = "xxx"; //!! username
$dbpass = "xxx"; //!! password

$mail = addslashes($_GET['email']);
$pass = addslashes($_GET['password']);

mysql_connect($dbhost, $dbuser, $dbpass)or die("3");
$verb = mysql_select_db($dbname);

if ($mail == null) {
die("No email specified, script terminated.");
}

if ($verb)
{
    $sql = "SELECT email_address, password, username FROM users WHERE email_address='".$mail."'";
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
    $passwort = $row->password;
    if ($passwort == $pass)
    {
    echo("1");
    exit();
        }
        else
        {
echo("0");
    exit();
        }
    }
}

mysql_close();
?>
