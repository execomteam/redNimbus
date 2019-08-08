import React from 'react'
import axios from 'axios';

export default class RegistrationForm extends React.Component{

    constructor(props){
        super(props);
        this.state = {firstName : '',lastName : '', email : '', phoneNumber : '', password : '', repeatedPassword : ''};

        this.handleFirstNameChange          = this.handleFirstNameChange.bind(this);
        this.handleLastNameChange           = this.handleLastNameChange.bind(this);
        this.handleEmailChange              = this.handleEmailChange.bind(this);
        this.handlePasswordChange           = this.handlePasswordChange.bind(this);
        this.handleRepeatedPasswordChange   = this.handleRepeatedPasswordChange.bind(this);
        this.handlePhoneNumberChange        = this.handlePhoneNumberChange.bind(this);
        this.handleSubmit                   = this.handleSubmit.bind(this);
    }

    handleFirstNameChange(event){
        this.setState({firstName : event.target.value});
    }

    handleLastNameChange(event){
        this.setState({lastName : event.target.value});
    }

    handleEmailChange(event){
        this.setState({email : event.target.value});
    }

    handlePhoneNumberChange(event){
        this.setState({phoneNumber : event.target.value});
    }

    handlePasswordChange(event){
        this.setState({password : event.target.value});
    }

    handleRepeatedPasswordChange(event){
        this.setState({repeatedPassword : event.target.value});
    }

    handleSubmit(event){
        //TODO:Check user input i.e. password == repeatedPassword...etc.
        this.SendRegisterRequest();
        event.preventDefault();
    }

    SendRegisterRequest(){
        let firstName   = this.state.firstName;
        let lastName    = this.state.lastName;
        let email       = this.state.email;
        let password    = this.state.password;
        let phoneNumber = this.state.phoneNumber;

        axios.post("http://localhost:59746/api/user/register",{firstName:firstName,lastName:lastName,email:email,passord:password}).then(function(response){console.log("response")});
    }

    render(){
        return(
            <form onSubmit={this.handleSubmit}>
                <table>
                    <tr><td><h3>Register</h3></td></tr>
                    <tr><td>First Name:</td>        <td><input type="text"       value={this.state.firstName}           onChange={this.handleFirstNameChange}           placeholder="First Name"       required/><br></br></td></tr>
                    <tr><td>Last Name:</td>         <td><input type="text"       value={this.state.lastName}            onChange={this.handleLastNameChange}            placeholder="Last Name"        required/><br></br></td></tr>
                    <tr><td>E-mail:</td>            <td><input type="email"      value={this.state.email}               onChange={this.handleEmailChange}               placeholder="E-mail adress"    required/><br></br></td></tr>
                    <tr><td>Password:</td>          <td><input type="password"   value={this.state.password}            onChange={this.handlePasswordChange}            placeholder="Password"         required/><br></br></td></tr>
                    <tr><td>Repeat Password:</td>   <td><input type="password"   value={this.state.repeatedPassword}    onChange={this.handleRepeatedPasswordChange}    placeholder="Reapeat Password" required/><br></br></td></tr>
                    <tr><td>Phone Number:</td>      <td><input type="text"       value={this.state.phoneNumber}         onChange={this.handlePhoneNumberChange}         placeholder="Phone Number"     required/><br></br></td></tr>
                    <tr><td><input type="submit" value="Sign Up"/></td></tr>
                </table>
            </form>
        )
    }

    
}