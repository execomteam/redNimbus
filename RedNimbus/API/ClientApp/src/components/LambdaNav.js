import React from 'react';
import CreateLambda from './CreateLambda'
import { Button } from "react-bootstrap";

class LambdaNav extends React.Component {
    render() {
        return (

            <div>
                <CreateLambda
                    addLambda={this.props.addLambda}
                    show={this.props.createModalShow}
                    onHide={this.props.setCreateModalShow}
                    path={this.props.path}
                />
            </div>
        );
    }
}

export default LambdaNav;
