<?php
if (isset($_REQUEST['email']))
//if "email" is filled out, send email
  {
  //send email
  $email = $_REQUEST['email'] ;
  $subject = $_REQUEST['subject'] ;
  $message = $_REQUEST['message'] ;
  $localemail = "linesman@esagamer.com";
  mail($localemail, $subject,
  $message, "From:" . $email);
  echo "0";
  }
else
//if "email" is not filled out, display the form
  {
die('Invalid.');
  }
?>