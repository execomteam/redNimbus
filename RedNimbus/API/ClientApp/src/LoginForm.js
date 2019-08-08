import React from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios';

class LoginForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = { email: '', password: '' };

        this.handleEmailChange = this.handleEmailChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleEmailChange(event) {
        this.setState({ email: event.target.value, password: this.state.password });
    }

    handlePasswordChange(event) {
        this.setState({ email: this.state.email, password: event.target.value });
    }

    handleSubmit(event) {
        const user = {
            email: this.state.email,
            password: this.state.password
        };
        axios.post('../api/login', { user }).then(response => { console.log(response) });
        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <table>
                    <tr><td>E-mail:</td>
                        <td><input type="text" value={this.state.email} onChange={this.handleEmailChange} /></td></tr>
                    <tr><td>Password:</td>
                        <td><input type="password" value={this.state.password} onChange={this.handlePasswordChange} /></td></tr>
                    <tr><td><input type="submit" value="Submit" /></td></tr>
                </table>
            </form>
        );
    }
}

export default LoginForm;
