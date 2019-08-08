import React, {useState} from 'react';
import LoginForm from './LoginForm';
import RegistrationForm from './RegistrationForm'
import SwitchButton from './SwitchButton'
import NavBar from './components/Router'




export default class App extends React.Component{
  constructor(props){
    super(props);
  }

  render(){
    return(

      <div>
        <NavBar/>
      </div>
    );
  }
}

