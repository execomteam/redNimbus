import React from 'react';
import axios from 'axios';
import { withRouter } from "react-router-dom";
import './css/LoginForm.css'

class LoginForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = { email: '', password: '' };

        this.redirectToRegistration = this.redirectToRegistration.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleResponse = this.handleResponse.bind(this);
        this.handleError = this.handleError.bind(this);
    }

    handleResponse(resp) {
        let self = this;
        axios.post('http://localhost:65001/api/user/get', { key: resp.data.key }).then(
            (response) => { self.props.changeState(response) },
            (response) => { self.handleError(response) }                                                                        
        );
        this.props.history.push("/");
    }

    redirectToRegistration() {
        this.props.history.push("/register");
    }

    handleError(resp) {
        alert('Error');
    }

    sendRequest() {
        let self = this;
        axios.post('http://localhost:65001/api/user/authenticate', { email: this.state.email, password: this.state.password }).then(
            (response) => { self.handleResponse(response) },
            (response) => { self.handleError(response) }
        );
    }

    handleSubmit(event) {
        event.preventDefault();
        this.setState({
            email: document.getElementById('email').value,
            password: document.getElementById('password').value
        }, this.sendRequest);
    }

    render() {
        return (
            <div className="global-container">
                <div className="card login-form">
                    <div className="card-body">
                        <h1 className="card-title text-center">Log in to redNimbus</h1>
                        <div className="card-text">

                            <form onSubmit={this.handleSubmit}>
                                <div className="form-group">
                                    <label htmlFor="email">E-mail</label>
                                    <input type="email"
                                           className="form-control form-control-sm"
                                           id="email"
                                           placeholder = "Enter e-mail"
                                           required
                                           />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="password">Password</label>
                                    <input type="password"
                                    className="form-control form-control-sm"
                                           id="password"
                                           placeholder = "Enter password"
                                           required
                                           />
                                </div>

                                <button type="submit" className="btn btn-primary btn-block">Sign in</button>
                                <button className="btn btn-block" value="Cancel" onClick={this.redirectToRegistration}>Sign Up</button>
                                
                            </form>
                            
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default withRouter(LoginForm);
