import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

class CreateNewBucket extends React.Component {
    constructor(props){
        super(props);
        this.createNewBucket = this.createNewBucket.bind(this);
    }

    createNewBucket(event)
    {
        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        var bucketName = document.getElementById("newBucketName").value;
        var pth = this.props.path;

        axios.post("http://localhost:65001/api/bucket/createBucket", {Path: pth, Value: bucketName}, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp){
        this.props.addNewBucket(resp.data);
        this.props.onHide();
    }

    render(){
        return (
            <div>
                <Button variant="primary" onClick={()=>this.props.onHide(true)}>
                    Create new {this.props.type}
                </Button>
                <Modal
                show={this.props.show}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter"
                centered
                >
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            New {this.props.type}
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
                            <Button onClick={()=>this.props.onHide(false)}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>
        );
    }
}

export default CreateNewBucket;