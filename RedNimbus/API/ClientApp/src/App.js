import React, {useState} from 'react';
import RegisterForm, {Messages ,WelcomePage, sayHelloServer} from './RegistrationForm';
import LoginForm from './LoginForm';
import axios from 'axios';




function App(){

  var odgovor = "";
  var ime = "Emil"
  var userLastName = "Nisner Bajin"
  
  return(
    <div id="1">
          <WelcomePage userName = {ime} userSurname = {userLastName}/>
          <Messages messages="Password empty" />
          <LoginForm/>
          <RegisterForm/>
          <sayHelloServer odgovor/>
          <h1>{odgovor}</h1>
          <button></button>
    </div>
  );
}

export default App;