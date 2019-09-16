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
    }

    handleSubmit(event){
        let token = localStorage.getItem("token");

        let headers = {
            "Content-Type":'application/json; charset=utf-8',
            "token" : token

        }

        axios.post("http://localhost:65001/api/user/deactivateUserAccount", {} , {"headers": {token:token}}).then(
        (resoponse) => {
            this.props.history.push("/");
        },
        (response) => {
            this.setState({deactivateErrorMessage : response.data.message});
        }
        );
    }

    render(){
        return(
            <div className = "global-container">
                <center>
                    <h5>Warning</h5>
                    <h6>By deactivating your account you won't be able to access your files!</h6>
                    <form onSubmit={this.handleSubmit}>
                        <input type="submit" value="Deactivate Account"/>
                    </form>
                </center>
            </div>
        )
       
    }


}

export default DeactivateAccount;