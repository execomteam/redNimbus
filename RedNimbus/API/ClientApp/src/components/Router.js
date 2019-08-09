import React from 'react'
import { BrowserRouter as Router, Route, Link , withRouter} from "react-router-dom";
import LoginForm from './LoginForm'
import RegistrationForm from './RegistrationForm'
import Home from './Home'
import SignOut from './SignOut'

class NavBar extends React.Component {
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

    signOut = () => {
        this.setState({
            isLoggedIn:false
        });
        localStorage.clear();
    }

    render() {
        
        if(this.state.isLoggedIn)
            return (
                <div>
                    <Router>
                        <div>
                            <ul>
                                <li>
                                    <Link to="/">Home</Link>
                                </li>
                                <li>
                                    <Link to="/signout">Signout</Link>
                                </li>
                            </ul>

                            <hr />
                            <Route exact path="/" render={(props) => <Home user={this.state} />} />
                            <Route path="/signout" render={(props) => <SignOut signOut={this.signOut} />} />
                        </div>
                    </Router>
                </div>
            );

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

  export default withRouter(NavBar);

