import React from 'react';
import axios from 'axios';
import { withRouter } from "react-router-dom";
import './css/RegistrationForm.css'


class DeactivateAccount extends React.Component{
    constructor(props){
        super(props);
        this.setState =({
            deactivateErrorMessage : ''
        })

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    handleSubmit(event){
        event.preventDefault();
        let token = localStorage.getItem("token");

        let headers = {
            token:token
        }

        let self = this;

        axios.post("http://localhost:65001/api/user/deactivateUserAccount", {} , {"headers": headers}).then(
        (resoponse) => {
            localStorage.setItem('token','');
            self.props.signOut();
            self.props.history.push("/");
        },
        (response) => {
            self.setState({deactivateErrorMessage : response.data.message});
        }
        );
    }

    handleCancel(event){
        event.preventDefault();
        this.props.history.push("/");
    }


    render(){
        return(
            <div className = "global-container">
                <center>
                    <h3>Warning</h3>
                    <h6><label style={{ color: 'red' }}>By deactivating your account you won't be able to access your files! <br/> You will be logged out if you continue deactivation</label></h6>
                    <form onSubmit={this.handleSubmit}>
                        <input type="submit" value="I uderstand. Deactivate my Account."/><br/><br/>
                        <input type="button" value="I changed my mind. Take me back to safety." onClick={this.handleCancel}/>
                    </form>
                </center>
            </div>
        )
       
    }


}

export default withRouter(DeactivateAccount);