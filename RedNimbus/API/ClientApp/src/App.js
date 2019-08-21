import React from 'react';
import NavBar from './components/Router'
import axios from 'axios';

export default class App extends React.Component{

  constructor(props) {
      super(props);
      if (localStorage.getItem('token') === null ){
          this.state = {
              isLoggedIn: false,
              firstName: '',
              lastName: '',
              email: '',
              id: '',
              key: ''
          };
      } else {
          let token = localStorage.getItem('token');
          let self = this;
          this.state = {
              isLoggedIn: false
          }
          axios.post('http://localhost:65001/api/user/get', { key: token }).then(
              (response) => { self.changeState(response) },
              (response) => { self.handleError(response) }
          );
      }
  }

    handleError(response) {
        alert("Error");
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
      //alert(localStorage.getItem('token'));
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