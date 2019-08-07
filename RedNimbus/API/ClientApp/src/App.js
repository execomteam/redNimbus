import React, {useState} from 'react';
import RegisterForm, {Messages ,WelcomePage} from './RegistrationForm';


function App(){
  
  return(
    <div id="1">
      <WelcomePage userName="Emil" userSurname="Nisner Bajin"/>
      <Messages messages="Password empty"/>
      <RegisterForm/>
    </div>
  );
}

export default App;