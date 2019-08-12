import React from 'react'
import { withRouter } from "react-router-dom";

class SignOut extends React.Component{
    constructor(props) {
        super(props);
        this.setAnswerToYes = this.setAnswerToYes.bind(this);
        this.setAnswerToNo = this.setAnswerToNo.bind(this);
    }

    setAnswerToYes() {
        this.props.signOut();
        this.props.history.push('/');
    }

    setAnswerToNo() {
        this.props.history.push('/');
    }

    render(){
        return (
            <div className="global-container">
                <div className="card login-form">
                    <div className="card-body">
                        <h3 className="card-title text-center">Are you sure that you want to sign out?</h3>
                        <button className="btn btn-primary btn-block" onClick={this.setAnswerToYes}>Yes</button>
                        <button className="btn btn-block" onClick={this.setAnswerToNo}>No</button>
                    </div>
                </div>
            </div>

        )

    }

}

export default withRouter(SignOut);