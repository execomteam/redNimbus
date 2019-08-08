import React from 'react';
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
        const userLogin = {
            Email: this.state.email,
            Password: this.state.password
        };
        axios.post('http://localhost:59746/api/user/login', {Email:this.state.email , Password:this.state.password}).then(response => { console.log(response) });
        event.preventDefault();
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <table>
                    <tbody>
                        <tr><td><h3>Login</h3></td></tr>
                        <tr><td>E-mail:</td><td><input type="email"     value={this.state.email}    onChange={this.handleEmailChange}       placeholder = "E-mail Adress"   required/></td></tr>
                        <tr><td>Password:</td><td><input type="password"  value={this.state.password} onChange={this.handlePasswordChange}    placeholder = "Password"        required /></td></tr>
                        <tr><td><input type="submit"    value="Sign In"/></td></tr>
                        </tbody>
                </table>
            </form>
        );
    }
}

export default LoginForm;
