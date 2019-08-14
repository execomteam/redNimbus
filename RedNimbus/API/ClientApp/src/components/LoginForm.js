import React from 'react';
import axios from 'axios';
import { withRouter } from "react-router-dom";
import './css/LoginForm.css'

class LoginForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = { email: 'enisnerbajin@gmail.com', password: '@Testiranje97' };

        this.handleEmailChange = this.handleEmailChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.redirectToRegistration = this.redirectToRegistration.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleResponse = this.handleResponse.bind(this);
        this.handleError = this.handleError.bind(this);
    }

    handleEmailChange(event) {
        this.setState({ email: event.target.value, password: this.state.password });
    }

    handlePasswordChange(event) {
        this.setState({ email: this.state.email, password: event.target.value });
    }
    
    handleResponse(resp) {
        this.props.changeState(resp);
        this.props.history.push("/");
    }

    redirectToRegistration() {
        this.props.history.push("/register");
    }

    handleError(resp) {
        alert('Error');
    }

    handleSubmit(event) {
        
        let self=this;
        axios.post('http://localhost:65001/api/user/authenticate', { email: this.state.email, password: this.state.password }).then(
            (response)=>{self.handleResponse(response)}, 
            (response)=>{self.handleError(response)}

        );
        event.preventDefault();
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
                                           value={this.state.email}
                                           onChange={this.handleEmailChange}
                                           placeholder = "Enter e-mail"
                                           required
                                           />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="password">Password</label>
                                    <input type="password"
                                    className="form-control form-control-sm"
                                           id="password"
                                           value={this.state.password}
                                           onChange={this.handlePasswordChange}
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
