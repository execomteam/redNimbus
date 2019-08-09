import React from 'react'
import { withRouter } from "react-router-dom";

class Home extends React.Component {
    

    render(){
        if (this.props.user.isLoggedIn) {
            return (
                <div>
                    <h2>Welcome {this.props.user.firstName}</h2>
                </div>
            )
        }
        return (
            <div>
                <h2>Home</h2>
            </div>
        );
    }
}

export default withRouter(Home);