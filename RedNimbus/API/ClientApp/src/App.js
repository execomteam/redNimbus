import React from 'react';
import NavBar from './components/Router'

export default class App extends React.Component{

  constructor(props) {
      super(props);
      this.state = {
        isLoggedIn: false,
        firstName:'',
        lastName:'',
        email:'',
        id:'',
        key:''
    };
  }

  changeState = (resp) => {
    this.setState({
        isLoggedIn: true,
        firstName: resp.data.firstName,
        lastName: resp.data.lastName,
        email: resp.data.email,
        id: resp.data.id,
        key: resp.data.key
    });
    localStorage.setItem('token', this.state.key);
} 

  signOut = () => {
    this.setState({
        isLoggedIn:false
    });
    localStorage.clear();
  }

  render(){
    return(
      <div>
        <NavBar changeState={this.changeState} signOut={this.signOut} state={this.state}/>
      </div>
    );
  }
}