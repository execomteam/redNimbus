import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

class DeleteBucket extends React.Component {
    constructor(props) {
        super(props);
        this.deleteBucket = this.deleteBucket.bind(this);
    }

    deleteBucket(event) {
        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        var bucketName = document.getElementById("BucketName").value;

        axios.post("http://localhost:65001/api/bucket/deleteBucket", { Value: bucketName }, options).then(
            (resp) => this.onSuccessHandler(resp),
            (resp) => this.onErrorHandler(resp)
        );
    }

    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp) {
        this.props.deletingBucket(resp.data);
        this.props.onHide();
    }

    render() {
        return (
            <div>
                <Button variant="primary" onClick={() => this.props.onHide(true)}>
                    Delete a bucket
                </Button>
                <Modal
                    show={this.props.show}
                    size="lg"
                    aria-labelledby="contained-modal-title-vcenter"
                    centered
                >
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            Delete a Bucket
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <form onSubmit={this.deleteBucket}>
                            <div className="form-group">
                                <input type="text"
                                    className="form-control form-control-sm"
                                    id="BucketName"
                                    name="someBucket"
                                    placeholder="Enter name"
                                    required
                                />
                            </div>
                            <Button type="submit">Delete</Button>
                            <Button onClick={() => this.props.onHide(false)}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>
        );
    }
}

export default DeleteBucket;