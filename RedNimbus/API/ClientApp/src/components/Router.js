import React from 'react'
import {Route, withRouter, Switch } from "react-router-dom";
import LoginForm from './LoginForm'
import RegistrationForm from './RegistrationForm'
import Home from './Home'
import SignOut from './SignOut'
import Bucket from './Bucket'
import LambdaPage from './LambdaPage'
import DeactivateAccount from './DeactivateAccount'

class Routes extends React.Component {
    
    constructor(props){
        super(props);
    }

    render(){
            return (<Switch>
                <Route exact path="/"               render={(props) => <Home user={this.props.user} />} />
                <Route path="/lambda"               render={(props) => <LambdaPage path={this.props.path} />} />
                <Route path="/bucket"               render={(props) => <Bucket  name={this.props.user.firstName} />} />
                <Route path="/signout"              render={(props) => <SignOut signOut={this.props.signOut} />} />
                <Route path="/login"                render={(props) => <LoginForm changeState={this.props.changeState} />} />
                <Route path="/register"             render={(props) => <RegistrationForm />} />
                <Route path="/deactivateAccount"    render={(props) => <DeactivateAccount signOut={this.props.signOut}/>} />
            </Switch>
            );
        }
  }

  export default withRouter(Routes);

