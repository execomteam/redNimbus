import React from 'react'
import { withRouter } from "react-router-dom";
import './css/Home.css'

class Home extends React.Component {
    
    constructor(props){
        super(props);
    }

    render(){
        if (this.props.user.isLoggedIn) {
            return (
                <div className="global-container">
                    <div className="card register-form">
                        <div className="card-body">
                            <h1 className="card-title text-center">Welcome {this.props.user.firstName}</h1>
                        </div>
                    </div>
                </div>
            )
        }
        return (
            <div className="global-container">
                <div className="card register-form">
                    <div className="card-body">
                        <h1 className="card-title text-center">Home</h1>
                    </div>
                </div>
            </div>
        );
    }
}

export default withRouter(Home);