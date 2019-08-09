import React from 'react'
import axios from 'axios';
import { withRouter } from "react-router-dom";
import './css/RegistrationForm.css'

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
        this.redirectToHome = this.redirectToHome.bind(this);
        this.handleError = this.handleError.bind(this);
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

    handleError(resp) {
        alert('Error');
    }

    SendRegisterRequest(){
        let firstName   = this.state.firstName;
        let lastName    = this.state.lastName;
        let email       = this.state.email;
        let password    = this.state.password;
        let phoneNumber = this.state.phoneNumber;
        
        let self = this;
        


        axios.post("http://localhost:65001/api/user",{firstName:firstName,lastName:lastName,email:email,password:password, phoneNumber:phoneNumber}).then(function(response){
                self.props.history.push("/login");
            },
            (response) => { self.handleError(response) }
        );

    }

    render(){       
        return(
            <div className="global-container">
                <div className="card register-form">
                    <div className="card-body">
                        <h1 className="card-title text-center">Register</h1>
                        <div className="card-text">

                            <form onSubmit={this.handleSubmit}>

                                <div className="form-group">
                                    <label htmlFor="firstName">First Name</label>
                                    <input type="text"
                                           className="form-control form-control-sm"
                                           id="firstName"
                                           value={this.state.firstName}
                                           onChange={this.handleFirstNameChange}
                                           placeholder="Enter First Name"
                                           required
                                           />
                                </div>

                                <div className="form-group">
                                    <label htmlFor="lastName">Last Name</label>
                                    <input type="text"
                                           className="form-control form-control-sm"
                                           id="lastName"
                                           value={this.state.lastName}
                                           onChange={this.handleLastNameChange}
                                           placeholder="Enter Last Name"
                                           required
                                           />
                                </div>

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

                                <div className="form-group">
                                    <label htmlFor="repeatedPassword">Confirm password</label>
                                    <input type="password"
                                           className="form-control form-control-sm"
                                           id="repeatedPassword"
                                           value={this.state.repeatedPassword}
                                           onChange={this.handleRepeatedPasswordChange}
                                           placeholder = "Confirm password"
                                           required
                                           />
                                </div>

                                <div className="form-group">
                                    <label htmlFor="phoneNumber">Phone Number</label>
                                    <input type="text"
                                           className="form-control form-control-sm"
                                           id="phoneNumber"
                                           value={this.state.phoneNumber}
                                           onChange={this.handlePhoneNumberChange}
                                           placeholder=""
                                           />
                                </div>

                                <div>
                                    <span><input type='checkbox'
                                                name='tosCheckbox'
                                                id='tosCheckbox'
                                                checked={this.state.tosCheckbox}
                                                onChange={this.handleToSCheckboxChange} />I agree to the Terms of Service</span>
                                </div>

                                <button type="submit" className="btn btn-primary btn-block" value="Sign Up" disabled={!this.state.tosCheckbox}>Sign Up</button>
                                <button className="btn btn-block" value="Cancel" onClick={this.redirectToHome}>Cancel</button>
                            </form>
                            
                        </div>
                    </div>
                </div>
            </div>
        )
        
    }  
}

export default withRouter(RegistrationForm);