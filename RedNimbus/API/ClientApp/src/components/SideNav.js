import React from 'react';
import CreateNewBucket from './CreateNewBucket'
import UploadFile from './UploadFile'
import {Button } from "react-bootstrap";

class SideNav extends React.Component{
    /*
    <ul className="nav flex-column">
                    <li className="nav-item">
                        <a style={{hidden: true}} className="nav-link active" href="/bucket"></a>
                    </li>
                    <li className="nav-item">
                        <a style={{hidden: true}} className="nav-link" href="#"></a>
                    </li>
                </ul>
    */
    render(){
        return (
            <div>
                <div>
                <CreateNewBucket
                    addNewBucket={this.props.addNewBucket}
                    show={this.props.createModalShow}
                    onHide={this.props.setCreateModalShow}
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

 