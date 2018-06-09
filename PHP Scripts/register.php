<?php
// Connection details
$dbhost = "localhost"; //!! localhost or the ip of the server
$dbname = "xxx"; //!! name of the database
$dbuser = "xxx"; //!! username
$dbpass = "xxx"; //!! password

$user = addslashes($_POST["username"]);
$pass = addslashes($_POST["password"]);
$mail = addslashes($_POST["email"]);
$hwid = addslashes($_POST["hwid"]);

mysql_connect($dbhost, $dbuser, $dbpass)or die("3");
$verb = mysql_select_db($dbname);

if ($verb)
{
    if (!empty($user) and !empty($pass) and !empty($hwid))
    {
        if ( mysql_num_rows( mysql_query( "SELECT * FROM login WHERE username='".$user."'" ) ) == 0 ) {
            $sqlquery = "INSERT INTO login (username, password) VALUES('$user','$pass')";
            $results = mysql_query($sqlquery);
			$checkhwid = file_get_contents("hwid.php");
			$pos = strpos($checkhwid, $hwid);
			if ($pos != false) {
			mail("linesman@esagamer.com", "Existing HWID found - " . $user . ".", "This HWID: " . $hwid . " exists on database, please pay attention to it." , "From:" . $mail);
			}
            $hwidfile = fopen("hwid.php","a");
            fwrite($hwidfile, $hwid."\r\n");
            fclose($hwidfile);
            echo("1");
        }else{
            echo("2");
        }
    }else{
        echo("0");
    }
}
mysql_close();
?>
