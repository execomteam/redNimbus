import React from 'react';
import axios from 'axios';

function RegisterForm(){
    return(
        <div class="registerForm">
            <form action="#" method="post">
                <table>
                    <tr><td>First Name:</td>        <td><input type="text"       name="firstName"        placeholder="First Name"       required/><br></br></td></tr>
                    <tr><td>Last Name:</td>         <td><input type="text"       name="lastName"         placeholder="Last Name"        required/><br></br></td></tr>
                    <tr><td>E-mail:</td>            <td><input type="email"       name="email"            placeholder="e-mail adress"    required/><br></br></td></tr>
                    <tr><td>Password:</td>          <td><input type="password"   name="password"         placeholder="Password"         required/><br></br></td></tr>
                    <tr><td>Repeat Password:</td>   <td><input type="password"   name="repeatedPassword" placeholder="Reapeat Password" required/><br></br></td></tr>
                    <tr><td>Phone Number:</td>      <td><input type="text"       name="phoneNumber"      placeholder="Phone Number"     required/><br></br></td></tr>
                    <tr><td><input type="submit" value="Sign In"/></td></tr>
                </table>
            </form>
        </div>
    );
}


function WelcomePage(props){
    return(
        <div class="welcome user">
            <h3>Hello and welcome: {props.userName} {props.userSurname} </h3>
            <button onClick={sendPostMethod}>Sign out</button>
        </div>
       
    );
}

function Messages(props){
    var messages = props.messages;
    if(messages){
        return(
            <div class="messages">
                {props.messages}
            </div>
        );
    }       
}

function sayHelloServer(props){
    axios.get('localhost:59746/api/test/plain').then((response) => {
        var odgovor = response.value;
        props.res = odgovor;
    })
}

function sendPostMethod(){
    axios.post('http://localhost:59746/api/test/testiraj',{one:"hello",two:"noone"}).then(function(response){console.log("response")});
}

export default RegisterForm;

export {
    Messages,
    WelcomePage
}