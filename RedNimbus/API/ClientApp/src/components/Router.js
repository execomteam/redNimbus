import React from 'react'
import { BrowserRouter as Router, Route, Link , withRouter} from "react-router-dom";
import LoginForm from './LoginForm'
import RegistrationForm from './RegistrationForm'
import Home from './Home'
import SignOut from './SignOut'
import './css/NavBar.css'

class NavBar extends React.Component {

    render() {
        
        if(this.props.state.isLoggedIn)
            return (
                <div>
                    <Router>
                        <div>
                            <nav>
                                <ul>
                                    <li id="home"><Link to="/">Home</Link></li>
                                    <li id="signout"><Link to="/signout">Sign Out</Link></li>
                                </ul>
                            </nav>

                            <Route exact path="/" render={(props) => <Home user={this.props.state}/>} />
                            <Route path="/signout" render={(props) => <SignOut signOut={this.props.signOut} />} />
                        </div>
                    </Router>
                </div>
            );

        return (
            <div>
                <Router>
                    <div>
                        <nav>
                            <ul>
                                <li id="home"><Link to="/">Home</Link></li>
                                <li id="register"><Link to="/register">Sign Up</Link></li>
                                <li id="login"><Link to="/login">Sign In</Link></li>

                            </ul>
                        </nav>

                        <Route exact path="/" render={(props) => <Home user={this.props.state}/>} />
                        <Route path="/login" render={(props) => <LoginForm changeState={this.props.changeState} />} />
                        <Route path="/register" render={(props) => <RegistrationForm />} />
                    </div>
                </Router>
            </div>
        );
    }
  }

  export default withRouter(NavBar);

