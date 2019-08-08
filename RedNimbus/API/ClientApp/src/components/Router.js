import React from 'react'
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import LoginForm from '../LoginForm'
import RegistrationForm from '../RegistrationForm'

export default class NavBar extends React.Component {
    constructor(props){
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

    render(){
        return (
            <div>
                <Router>
                    <div>
                        <ul>
                            <li>
                                <Link to="/">Home</Link>
                            </li>
                            <li>
                                <Link to="/login">Login</Link>
                            </li>
                            <li>
                                <Link to="/register">Register</Link>
                            </li>
                        </ul>

                        <hr />

                        <Route exact path="/" render={(props) => <Home user={this.state}/>} />
                        <Route path="/login" render={(props) => <LoginForm changeState={this.changeState} />} />
                        <Route path="/register" render={(props) => <RegistrationForm />} />
                    </div>
                </Router>
            </div>
        );
    }
  }

function Home(props) {
    console.log(props);
    if (props.user.isLoggedIn) {
        return (
            <div>
                <h2>Welcome {props.user.firstName}</h2>
            </div>
        )
    }
    return (
        <div>
            <h2>Home</h2>
        </div>
    );
  }