import React from 'react'
import axios from 'axios';
import { withRouter } from "react-router-dom";

class RegistrationForm extends React.Component{

    constructor(props){
        super(props);
        this.state = {firstName : '',lastName : '', email : '', phoneNumber : '', password : '', repeatedPassword : '', tosCheckbox : false};

        this.handleFirstNameChange          = this.handleFirstNameChange.bind(this);
        this.handleLastNameChange           = this.handleLastNameChange.bind(this);
        this.handleEmailChange              = this.handleEmailChange.bind(this);
        this.handlePasswordChange           = this.handlePasswordChange.bind(this);
        this.handleRepeatedPasswordChange   = this.handleRepeatedPasswordChange.bind(this);
        this.handlePhoneNumberChange        = this.handlePhoneNumberChange.bind(this);
        this.handleToSCheckboxChange        = this.handleToSCheckboxChange.bind(this);
        this.handleSubmit                   = this.handleSubmit.bind(this);
        this.redirectToHome                 = this.redirectToHome.bind(this);
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

    handleToSCheckboxChange(event){
        this.setState({tosCheckbox : event.target.checked});
    }

    handleSubmit(event) {
        event.preventDefault();

        if (this.state.password === this.state.repeatedPassword)
            this.SendRegisterRequest();
        else
            alert('Passwords do not match');
    }

    redirectToHome(){
        this.props.history.push("/");
    }

    SendRegisterRequest(){
        let firstName   = this.state.firstName;
        let lastName    = this.state.lastName;
        let email       = this.state.email;
        let password    = this.state.password;
        let phoneNumber = this.state.phoneNumber;
        
        let self = this;
        


        axios.post("http://localhost:49307/api/user",{firstName:firstName,lastName:lastName,email:email,password:password, phoneNumber:phoneNumber}).then(function(response){
            self.props.history.push("/login");
        });

    }

    render(){       
        return(
            <form onSubmit={this.handleSubmit}>
                <table>
                    <tbody>
                        <tr>
                            <td>
                                <h3>Register</h3>
                            </td>
                        </tr>
                        <tr>
                            <td>First Name:</td>
                            <td><input type="text" value={this.state.firstName} onChange={this.handleFirstNameChange} placeholder="First Name" required/></td>
                        </tr>
                        <tr>
                            <td>Last Name:</td>
                            <td><input type="text"       value={this.state.lastName}            onChange={this.handleLastNameChange}            placeholder="Last Name"        required/></td>
                        </tr>
                        <tr>
                            <td>E-mail:</td>
                            <td><input type="email"      value={this.state.email}               onChange={this.handleEmailChange}               placeholder="E-mail adress"    required/></td>
                        </tr>
                        <tr>
                            <td>Password:</td>
                            <td><input type="password"   value={this.state.password}            onChange={this.handlePasswordChange}            placeholder="Password"         required/></td>
                        </tr>
                        <tr>
                            <td>Repeat Password:</td>
                            <td><input type="password"   value={this.state.repeatedPassword}    onChange={this.handleRepeatedPasswordChange}    placeholder="Reapeat Password" required/></td>
                        </tr>
                        <tr>
                            <td>Phone Number:</td>
                            <td><input type="text"       value={this.state.phoneNumber}         onChange={this.handlePhoneNumberChange}         placeholder="Phone Number"             /></td>
                        </tr>
                        <tr>
                            <td>
                                <span>
                                <input type='checkbox' name='tosCheckbox' id='tosCheckbox' checked={this.state.tosCheckbox} onChange={this.handleToSCheckboxChange} />
                                I agree to the Terms of Service
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="button" value="Cancel" onClick={this.redirectToHome}/>
                                <input type="submit" value="Sign Up" disabled={!this.state.tosCheckbox}/>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </form>
        )
        
    }  
}

export default withRouter(RegistrationForm);