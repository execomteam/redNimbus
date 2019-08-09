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
            <div>    
                <h4>Are you sure that you want to sign out?</h4>
                <button onClick={this.setAnswer}>Yes</button>
            </div>
        )

    }

}

export default withRouter(SignOut);