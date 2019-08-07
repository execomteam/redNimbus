import React, {useState} from 'react';
import RegisterForm, {Messages ,WelcomePage} from './RegistrationForm';
import LoginForm from './LoginForm';


function App(){
  
  return(
    <div id="1">
          <WelcomePage userName="Emil" userSurname="Nisner Bajin"/>
          <Messages messages="Password empty" />
          <LoginForm />
          <RegisterForm/>
    </div>
  );
}

export default App;