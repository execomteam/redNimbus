import React from 'react'
import { withRouter } from "react-router-dom";

class SignOut extends React.Component{
    constructor(props) {
        super(props);
        this.setAnswer = this.setAnswer.bind(this);
    }

    setAnswer() {
        this.props.signOut();
        this.props.history.push('/');
    }

    render(){
        return (
            <div className="global-container">
                <div className="card login-form">
                    <div className="card-body">
                        <h3 className="card-title text-center">Are you sure that you want to sign out?</h3>
                        <button className="btn btn-primary btn-block" onClick={this.setAnswer}>Yes</button>
                    </div>
                </div>
            </div>

        )

    }

}

export default withRouter(SignOut);