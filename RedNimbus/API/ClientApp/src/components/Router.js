import React from 'react'
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import LoginForm from '../LoginForm'
import RegistrationForm from '../RegistrationForm'
export default function NavBar() {
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
  
          <Route exact path="/" component={Home} />
          <Route path="/login" component={LoginForm} />
          <Route path="/register" component={RegistrationForm} />
        </div>
      </Router>
      </div>
    );
  }

  function Home() {
    return (
      <div>
        <h2>Home</h2>
      </div>
    );
  }