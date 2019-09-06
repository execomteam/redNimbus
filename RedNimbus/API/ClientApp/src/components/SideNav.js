import React from 'react';
import CreateNewBucket from './CreateNewBucket'
import UploadFile from './UploadFile'
import {Button } from "react-bootstrap";

class SideNav extends React.Component{
    constructor(props) {
        super(props);
        this.changePath = this.changePath.bind(this);
    }

    changePath() {
        let path = this.props.path;
        if (path != "/") {
            let indexOfSlash = path.lastIndexOf("/");
            path = path.slice(0, indexOfSlash);
            indexOfSlash = path.lastIndexOf("/");
            path = path.slice(0, indexOfSlash);
            path = path + "/";
            this.props.changePath(path);
        }
    }

    render(){
        return (
            <div>
                <div>
                    <Button onClick={this.changePath}>Go back</Button>
                </div>

                <div>
                <CreateNewBucket
                    addNewBucket={this.props.addNewBucket}
                    show={this.props.createModalShow}
                    onHide={this.props.setCreateModalShow}
                    path={this.props.path}
                />
                </div>
                <div>
                    <UploadFile
                        uploadFile={this.props.uploadFile}
                        path={this.props.path}
                        show={this.props.uploadModalShow}
                        onHide={this.props.setUploadModalShow}
                    />
                </div>
            </div>

                
        );
    }
}

export default SideNav;

 