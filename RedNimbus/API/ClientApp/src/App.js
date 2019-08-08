import React, {useState} from 'react';
import LoginForm from './LoginForm';
import RegistrationForm from './RegistrationForm'




export default class App extends React.Component{
  constructor(props){
    super(props);
  }

  render(){
    return(
      <div>
        <RegistrationForm/>
        <LoginForm/>
      </div>
    );
  }
}

