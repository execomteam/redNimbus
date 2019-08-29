import React from 'react';
import axios from 'axios';
import FolderGroup from './FolderGroup'
import FileGroup from './FileGroup'
import SideNav from './SideNav'
import { Modal, Button } from "react-bootstrap";


class Bucket extends React.Component
{
    constructor(props) {
        super(props);

        this.state = {
            folders: [],
            files: [],
            modalShow: false
        }

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        axios.get("http://localhost:65001/api/bucket", options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    setModalShow(value){
        this.setState({
            modalShow: value
        });
    }

    onErrorHandler(response){
        //will see
    }

    onSuccessHandler(resp){
        var tempFolders = [];
        var tempFiles = [];
        var flag = true;

        for (var i = 0; i < resp.data.length; i++) {
            if (resp.data[i] === '*') {
              flag = false;
              continue;
            }
            if(flag==true){
                tempFolders.push(resp.data[i])
            }else{
                tempFiles.push(resp.data[i]);
            }
        }
        this.setState({
            folders: tempFolders,
            files: tempFiles
        });
    }

    onClickeCreateNewBucket(){
        
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2">
                        <SideNav onClick={this.onClickeCreateNewBucket}/>
                    </div>
                    <div className="col-md-10">   
                        <br />
                        <FolderGroup content={this.state.folders}/>
                        <hr/>
                        <FileGroup content={this.state.files}/>
                    </div>
                </div>

                    <Button variant="primary" onClick={() => this.setModalShow(true)}>
                        Launch vertically centered modal
                    </Button>

                    <MyVerticallyCenteredModal
                        show={this.state.modalShow}
                        onHide={() => this.setModalShow(false)}
      />

            </div>
        );
    }
}

class MyVerticallyCenteredModal extends React.Component {
    constructor(props){
        super(props);
        this.createNewBucket = this.createNewBucket.bind(this);
    }

    createNewBucket(data)
    {
        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        var x = document.getElementById("newBucketName").value;

        axios.post("http://localhost:65001/api/bucket/createBucket", {Value: x}, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onSuccessHandler(resp){
        alert('usaooo');
    }

    render(){
        return (
            <Modal
            show={this.props.show}
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered
            >
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                New Bucket
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form onSubmit={this.createNewBucket}>
                    <div className="form-group">
                        <input type="text"
                        className="form-control form-control-sm"
                                id="newBucketName"
                                name="newBucket"
                                placeholder = "Enter name"
                                required
                        />
                    </div>
                    <Button type="submit">Create</Button>
                    <Button onClick={this.props.onHide}>Close</Button>
                </form>
            </Modal.Body>
            </Modal>
        );
    }
  }

export default Bucket;